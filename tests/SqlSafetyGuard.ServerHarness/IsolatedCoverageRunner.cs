using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record IsolatedFixture(string Id, string[] StatementTypes, string SetupSql, string Sql,
    string ExpectedVerdict, string VerifySql, JsonElement BeforeScalar, JsonElement AfterScalar, string CleanupSql, bool SessionOnly = false, bool BulkCopy = false, bool ObserveChange = false);
internal sealed record IsolatedFixtures(int SchemaVersion, IsolatedFixture[] Fixtures);

internal static class IsolatedCoverageRunner
{
    public static int Run(Args options, AnalyzerHost analyzer)
    {
        var corpus = JsonSerializer.Deserialize<IsolatedFixtures>(File.ReadAllText(options.IsolatedCoverage!), CoverageCorpus.Json)!;
        if (corpus.SchemaVersion != 1 || corpus.Fixtures.Length == 0 || corpus.Fixtures.Select(fixture => fixture.Id).Distinct().Count() != corpus.Fixtures.Length)
            throw new InvalidDataException("Invalid isolated fixture corpus.");
        foreach (var fixture in corpus.Fixtures)
        {
            var kinds = CoverageCorpus.StatementTypes(fixture.Sql);
            CoverageCorpus.StatementTypes(fixture.SetupSql);
            CoverageCorpus.StatementTypes(fixture.VerifySql);
            CoverageCorpus.StatementTypes(fixture.CleanupSql);
            if (string.IsNullOrWhiteSpace(fixture.Id) || string.IsNullOrWhiteSpace(fixture.Sql) || string.IsNullOrWhiteSpace(fixture.VerifySql) ||
                !new[] { "ReadOnly", "TempOnly", "RealWrite", "Unknown" }.Contains(fixture.ExpectedVerdict) ||
                fixture.BeforeScalar.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                fixture.AfterScalar.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                fixture.StatementTypes.Length == 0 || fixture.StatementTypes.Any(kind => !kinds.Contains(kind)) ||
                (fixture.SessionOnly && fixture.StatementTypes.Any(kind => kind != "SetUserStatement" && kind != "UseStatement")))
                throw new InvalidDataException("Incorrect isolated kind link: " + fixture.Id);
        }
        if (options.ValidateCoverage) return 0;
        var container = Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONTAINER") ?? "";
        if (!Regex.IsMatch(container, "^sqlguard-cov-[a-f0-9]{16}$"))
            throw new InvalidOperationException("A run-owned disposable Docker container is required.");
        var process = Process.Start(new ProcessStartInfo("docker")
        {
            ArgumentList = { "inspect", container },
            RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true
        })!;
        var inspection = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new InvalidOperationException("Container ownership label verification failed.");
        using var inspected = JsonDocument.Parse(inspection);
        var owned = inspected.RootElement[0];
        if (owned.GetProperty("Config").GetProperty("Labels").GetProperty("sqlguard.coverage").GetString() != "disposable" ||
            owned.GetProperty("Mounts").GetArrayLength() != 0)
            throw new InvalidOperationException("Disposable ownership label and absence of host mounts required.");
        var builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("SQLGUARD_COVERAGE_CONNECTION") ?? "")
        { InitialCatalog = "master", TrustServerCertificate = true, Pooling = false, ConnectTimeout = 15 };
        using var admin = new SqlConnection(builder.ConnectionString);
        admin.Open();
        if (Convert.ToString(Scalar(admin, "SELECT CONVERT(nvarchar(128), SERVERPROPERTY('MachineName'));", options.Timeout)) != container)
            throw new InvalidOperationException("SQL Server is not the verified disposable container.");
        var startedUtc = DateTimeOffset.UtcNow;
        var analyzerSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(analyzer.AssemblyPath)));
        var harnessSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(typeof(IsolatedCoverageRunner).Assembly.Location)));
        var fixturesSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(options.IsolatedCoverage!)));
        var results = new List<object>();
        var failed = false;
        foreach (var fixture in corpus.Fixtures.Where(fixture => options.Filter == null || fixture.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase)))
        {
            var database = "SqlGuard_IsolatedCase";
            var verdict = analyzer.Analyze(fixture.Sql);
            string? error = null;
            string? before = null;
            string? after = null;
            var stage = "create";
            try
            {
                Execute(admin, $"CREATE DATABASE [{database}];", options.Timeout);
                builder.InitialCatalog = database;
                using var connection = new SqlConnection(builder.ConnectionString);
                connection.Open();
                stage = "setup";
                Execute(connection, fixture.SetupSql, options.Timeout);
                before = CoverageCorpus.Scalar(Scalar(connection, fixture.VerifySql, options.Timeout));
                stage = "execute";
                if (fixture.BulkCopy)
                {
                    if (fixture.StatementTypes.Length != 1 || fixture.StatementTypes[0] != "InsertBulkStatement") throw new InvalidDataException("Bulk protocol requires InsertBulkStatement.");
                    using var bulk = new SqlBulkCopy(connection) { DestinationTableName = "dbo.BulkTarget", BulkCopyTimeout = options.Timeout };
                    var rows = new System.Data.DataTable();
                    rows.Columns.Add("Id", typeof(int));
                    rows.Rows.Add(1);
                    bulk.WriteToServer(rows);
                }
                else Execute(connection, fixture.Sql, options.Timeout);
                stage = "verify";
                after = CoverageCorpus.Scalar(Scalar(connection, fixture.VerifySql, options.Timeout));
                if ((fixture.ObserveChange ? before == after || before == "null" || after == "null" : before != CoverageCorpus.Scalar(fixture.BeforeScalar) || after != CoverageCorpus.Scalar(fixture.AfterScalar)) || verdict.Overall != fixture.ExpectedVerdict || verdict.Error != null)
                    throw new InvalidDataException($"Expected {fixture.BeforeScalar} -> {fixture.AfterScalar}, got {before} -> {after}; verdict {verdict.Overall}, expected {fixture.ExpectedVerdict}.");
                if (verdict.IsGreen && !fixture.SessionOnly && before != after)
                    throw new InvalidDataException("FALSE GREEN: verified state changed.");
                stage = "complete";
            }
            catch (Exception exception) { error = exception.GetType().Name + ": " + exception.Message; }
            finally
            {
                try
                {
                    Execute(admin, fixture.CleanupSql, options.Timeout);
                }
                catch (Exception exception) { error = (error ?? "") + "; fixture cleanup: " + exception.Message; }
                try
                {
                    Execute(admin, "IF DB_ID(N'SqlGuard_IsolatedRenamed') IS NOT NULL BEGIN ALTER DATABASE [SqlGuard_IsolatedRenamed] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [SqlGuard_IsolatedRenamed]; END; IF DB_ID(N'SqlGuard_IsolatedCase') IS NOT NULL BEGIN ALTER DATABASE [SqlGuard_IsolatedCase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [SqlGuard_IsolatedCase]; END;", options.Timeout);
                }
                catch (Exception exception) { error = (error ?? "") + "; cleanup: " + exception.Message; }
            }
            failed |= error != null;
            results.Add(new { fixture.Id, Fixture = fixture, Status = error == null ? "pass" : "execution-error-or-assertion-failure", Stage = stage, Before = before, After = after, Verdict = verdict, Error = error });
            var output = Path.Combine(options.Out, "isolated-results.json");
            File.WriteAllText(output + ".tmp", JsonSerializer.Serialize(new { StartedUtc = startedUtc, RunUtc = DateTimeOffset.UtcNow, AnalyzerSha256 = analyzerSha256, HarnessSha256 = harnessSha256, FixturesSha256 = fixturesSha256, Container = container, ServerVersion = admin.ServerVersion, Complete = false, Results = results }, CoverageCorpus.Json));
            File.Move(output + ".tmp", output, true);
            Console.WriteLine($"{fixture.Id}: {(error == null ? "pass" : error)}");
        }
        File.WriteAllText(Path.Combine(options.Out, "isolated-results.json"), JsonSerializer.Serialize(new { StartedUtc = startedUtc, RunUtc = DateTimeOffset.UtcNow, AnalyzerSha256 = analyzerSha256, HarnessSha256 = harnessSha256, FixturesSha256 = fixturesSha256, Container = container, ServerVersion = admin.ServerVersion, Complete = true, Results = results }, CoverageCorpus.Json));
        return failed || options.RequireComplete || results.Count == 0 ? 1 : 0;
    }

    private static object? Scalar(SqlConnection connection, string sql, int timeout)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = timeout };
        return command.ExecuteScalar();
    }

    private static void Execute(SqlConnection connection, string sql, int timeout)
    {
        var script = SqlCmdLite.Split(sql, sqlCmdMode: false);
        if (script.NotRunnable != null || script.Batches.Any(batch => batch.Error != null || batch.Repeat != 1))
            throw new InvalidDataException("Only plain SQL and nonrepeated GO batches are supported.");
        foreach (var batch in script.Batches)
        {
            using var command = new SqlCommand(batch.Sql, connection) { CommandTimeout = timeout };
            using var reader = command.ExecuteReader();
            do { while (reader.Read()) { } } while (reader.NextResult());
        }
    }
}
