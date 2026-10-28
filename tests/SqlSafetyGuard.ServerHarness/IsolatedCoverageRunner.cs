using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record IsolatedFixture(string Id, string[] StatementTypes, string SetupSql, string Sql,
    string ExpectedVerdict, string VerifySql, JsonElement BeforeScalar, JsonElement AfterScalar, string CleanupSql, bool SessionOnly = false, bool BulkCopy = false, bool ObserveChange = false, bool AttemptOnly = false, bool QueryNotification = false);
internal sealed record IsolatedFixtures(int SchemaVersion, IsolatedFixture[] Fixtures);

internal static class IsolatedCoverageRunner
{
    public static int Run(Args options, AnalyzerHost analyzer)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(options.IsolatedCoverage!));
        if (document.RootElement.GetProperty("schemaVersion").GetInt32() != 1)
            throw new InvalidDataException("Invalid isolated fixture schema version.");
        var accepted = new List<IsolatedFixture>();
        var rejected = new List<object>();
        var identifiers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var element in document.RootElement.GetProperty("fixtures").EnumerateArray())
        {
            try
            {
                var fixture = element.Deserialize<IsolatedFixture>(CoverageCorpus.Json) ?? throw new InvalidDataException("Null fixture.");
                if (fixture.StatementTypes == null || fixture.Sql == null || fixture.SetupSql == null || fixture.VerifySql == null || fixture.CleanupSql == null)
                    throw new InvalidDataException("Missing fixture fields.");
                var kinds = fixture.AttemptOnly ? [] : CoverageCorpus.StatementTypes(fixture.Sql.Replace("{{NotificationId}}", "1", StringComparison.Ordinal));
                if (!fixture.AttemptOnly)
                {
                    CoverageCorpus.StatementTypes(fixture.SetupSql);
                    CoverageCorpus.StatementTypes(fixture.VerifySql);
                    CoverageCorpus.StatementTypes(fixture.CleanupSql);
                }
                if (string.IsNullOrWhiteSpace(fixture.Id) || string.IsNullOrWhiteSpace(fixture.Sql) || string.IsNullOrWhiteSpace(fixture.VerifySql) ||
                    !new[] { "ReadOnly", "TempOnly", "RealWrite", "Unknown" }.Contains(fixture.ExpectedVerdict) ||
                    fixture.BeforeScalar.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                    fixture.AfterScalar.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                    fixture.StatementTypes.Length == 0 || (!fixture.AttemptOnly && fixture.StatementTypes.Any(kind => !kinds.Contains(kind))) ||
                    (fixture.SessionOnly && fixture.StatementTypes.Any(kind => kind != "SetUserStatement" && kind != "UseStatement")))
                    throw new InvalidDataException("Incorrect isolated kind link: " + fixture.Id);
                if (!identifiers.Add(fixture.Id)) throw new InvalidDataException("Duplicate fixture id: " + fixture.Id);
                accepted.Add(fixture);
            }
            catch (Exception exception) when (exception is InvalidDataException or ArgumentException or JsonException)
            {
                rejected.Add(new { Raw = element.GetRawText(), Error = exception.Message });
            }
        }
        Directory.CreateDirectory(options.Out);
        File.WriteAllText(Path.Combine(options.Out, "isolated-rejected.json"), JsonSerializer.Serialize(rejected, CoverageCorpus.Json));
        var corpus = new IsolatedFixtures(1, accepted.ToArray());
        if (corpus.Fixtures.Length == 0) throw new InvalidDataException("No usable isolated fixtures; inspect isolated-rejected.json.");
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
        var completed = new HashSet<string>(StringComparer.Ordinal);
        var output = Path.Combine(options.Out, "isolated-results.json");
        var image = owned.GetProperty("Image").GetString();
        if (File.Exists(output))
        {
            using var previous = JsonDocument.Parse(File.ReadAllText(output));
            var saved = previous.RootElement;
            if (saved.GetProperty("AnalyzerSha256").GetString() != analyzerSha256 ||
                saved.GetProperty("HarnessSha256").GetString() != harnessSha256 ||
                saved.GetProperty("FixturesSha256").GetString() != fixturesSha256 ||
                !saved.TryGetProperty("Image", out var previousImage) || previousImage.GetString() != image)
                throw new InvalidOperationException("Fixture output belongs to different binaries, inputs, or image.");
            foreach (var retained in saved.GetProperty("Results").EnumerateArray())
            {
                results.Add(retained.Clone());
                completed.Add(retained.GetProperty("Id").GetString()!);
            }
        }
        var failed = false;
        foreach (var fixture in corpus.Fixtures.Where(fixture => options.Filter == null || fixture.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase)).Take(options.Limit > 0 ? options.Limit : int.MaxValue))
        {
            if (completed.Contains(fixture.Id)) continue;
            var database = "SqlGuard_IsolatedCase";
            var serverName = Convert.ToString(Scalar(admin, "SELECT CONVERT(nvarchar(128),SERVERPROPERTY('ServerName'));", options.Timeout))!.Replace("'", "''");
            var sql = fixture.Sql.Replace("{{ServerName}}", serverName, StringComparison.Ordinal);
            var setupSql = fixture.SetupSql.Replace("{{ServerName}}", serverName, StringComparison.Ordinal);
            var verdict = analyzer.Analyze(sql);
            var notificationStarted = false;
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
                Execute(connection, setupSql, options.Timeout);
                if (fixture.QueryNotification)
                {
                    notificationStarted = SqlDependency.Start(builder.ConnectionString);
                    Execute(connection, "SET ANSI_NULLS ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON; SET ARITHABORT ON; SET CONCAT_NULL_YIELDS_NULL ON; SET QUOTED_IDENTIFIER ON; SET NUMERIC_ROUNDABORT OFF;", options.Timeout);
                    using var notificationQuery = new SqlCommand("SELECT Id,Value FROM dbo.NotifyTarget;", connection) { CommandTimeout = options.Timeout };
                    var dependency = new SqlDependency(notificationQuery);
                    dependency.OnChange += (_, _) => { };
                    using (var reader = notificationQuery.ExecuteReader()) while (reader.Read()) { }
                    var subscription = Convert.ToString(Scalar(connection, "SELECT TOP(1) id FROM sys.dm_qn_subscriptions WHERE database_id=DB_ID();", options.Timeout));
                    if (string.IsNullOrEmpty(subscription)) throw new InvalidDataException("Owned query notification was not registered.");
                    sql = sql.Replace("{{NotificationId}}", subscription, StringComparison.Ordinal);
                    verdict = analyzer.Analyze(sql);
                }
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
                else Execute(connection, sql, options.Timeout);
                stage = "verify";
                after = CoverageCorpus.Scalar(Scalar(connection, fixture.VerifySql, options.Timeout));
                if (!fixture.AttemptOnly && ((fixture.ObserveChange ? before == after || before == "null" || after == "null" : before != CoverageCorpus.Scalar(fixture.BeforeScalar) || after != CoverageCorpus.Scalar(fixture.AfterScalar)) || verdict.Overall != fixture.ExpectedVerdict || verdict.Error != null))
                    throw new InvalidDataException($"Expected {fixture.BeforeScalar} -> {fixture.AfterScalar}, got {before} -> {after}; verdict {verdict.Overall}, expected {fixture.ExpectedVerdict}.");
                if (!fixture.AttemptOnly && verdict.IsGreen && !fixture.SessionOnly && before != after)
                    throw new InvalidDataException("FALSE GREEN: verified state changed.");
                stage = "complete";
            }
            catch (Exception exception) { error = exception.GetType().Name + ": " + exception.Message; }
            finally
            {
                if (notificationStarted)
                    try { SqlDependency.Stop(builder.ConnectionString); }
                    catch (Exception exception) { error = (error ?? "") + "; notification cleanup: " + exception.Message; }
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
            results.Add(new { fixture.Id, Fixture = fixture, Sql = sql, Status = error == null ? fixture.AttemptOnly ? "executed-unverified" : "pass" : "execution-error-or-assertion-failure", Stage = stage, Before = before, After = after, Verdict = verdict, Error = error });
            File.WriteAllText(output + ".tmp", JsonSerializer.Serialize(new { StartedUtc = startedUtc, RunUtc = DateTimeOffset.UtcNow, AnalyzerSha256 = analyzerSha256, HarnessSha256 = harnessSha256, FixturesSha256 = fixturesSha256, Image = image, Container = container, ServerVersion = admin.ServerVersion, Complete = false, Results = results }, CoverageCorpus.Json));
            File.Move(output + ".tmp", output, true);
            Console.WriteLine($"{fixture.Id}: {(error == null ? fixture.AttemptOnly ? "executed-unverified" : "pass" : error)}");
        }
        failed |= results.Any(result => JsonSerializer.SerializeToElement(result).GetProperty("Error").ValueKind != JsonValueKind.Null);
        var complete = rejected.Count == 0 && corpus.Fixtures.All(fixture => results.Any(result => JsonSerializer.SerializeToElement(result).GetProperty("Id").GetString() == fixture.Id));
        File.WriteAllText(output, JsonSerializer.Serialize(new { StartedUtc = startedUtc, RunUtc = DateTimeOffset.UtcNow, AnalyzerSha256 = analyzerSha256, HarnessSha256 = harnessSha256, FixturesSha256 = fixturesSha256, Image = image, Container = container, ServerVersion = admin.ServerVersion, Complete = complete, Results = results }, CoverageCorpus.Json));
        return failed || rejected.Count != 0 || (options.RequireComplete && !complete) || results.Count == 0 ? 1 : 0;
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
