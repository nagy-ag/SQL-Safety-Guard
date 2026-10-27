using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SqlSafetyGuard.ServerHarness;

internal sealed class CoverageResult
{
    public required string Id { get; init; }
    public required CoverageFixture Fixture { get; init; }
    public required Verdict Verdict { get; init; }
    public string[] ParsedTypes { get; init; } = [];
    public string Status { get; set; } = "execution-error";
    public string Stage { get; set; } = "create-database";
    public string? Error { get; set; }
    public string? Database { get; set; }
    public string? ServerVersion { get; set; }
    public string? ActualScalar { get; set; }
    public int? TransactionCount { get; set; }
    public bool? VisibleChanged { get; set; }
    public bool? PersistedChanged { get; set; }
    public string? BeforeHash { get; set; }
    public string? VisibleHash { get; set; }
    public string? PersistedHash { get; set; }
    public string[] Difference { get; set; } = [];
}

internal static class CoverageRunner
{
    private sealed record RunIdentity(DateTimeOffset StartedUtc, string AnalyzerSha256, string HarnessSha256, string ManifestSha256, string FixturesSha256);

    private static string FileHash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    public static int Run(Args options, AnalyzerHost analyzer)
    {
        if (options.Keep || options.Tests.Count + options.Sql.Count + options.Samples.Count > 0)
            throw new ArgumentException("Strict coverage cannot be combined with --keep or discovery corpus options.");
        var identity = new RunIdentity(DateTimeOffset.UtcNow, FileHash(analyzer.AssemblyPath), FileHash(typeof(CoverageRunner).Assembly.Location),
            FileHash(Path.Combine(options.Coverage!, "manifest.json")), FileHash(Path.Combine(options.Coverage!, "fixtures.json")));
        var (manifest, allFixtures) = CoverageCorpus.Load(options.Coverage!);
        Console.WriteLine($"Validated {manifest.Entries.Length} kinds and {allFixtures.Length} explicit fixtures.");
        if (options.ValidateCoverage) return 0;
        var fixtures = allFixtures.Where(fixture => options.Filter == null || fixture.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase));
        if (options.Limit > 0) fixtures = fixtures.Take(options.Limit);
        var selected = fixtures.ToArray();
        if (selected.Length == 0) throw new ArgumentException("No coverage fixtures selected.");
        var results = new List<CoverageResult>();
        foreach (var fixture in selected)
        {
            var result = RunFixture(options, analyzer, fixture);
            results.Add(result);
            Write(options, identity, manifest, allFixtures, results);
            Console.WriteLine($"[{results.Count}/{selected.Length}] {fixture.Id}: {result.Status}, {result.Verdict.Overall}, visible={result.VisibleChanged}, persisted={result.PersistedChanged} {result.Error}");
        }
        var complete = manifest.Entries.All(entry => entry.FixtureIds.Length > 0 && entry.FixtureIds.All(id => results.Any(result => result.Id == id && result.Status == "pass")));
        Console.WriteLine($"Report: {Path.GetFullPath(Path.Combine(options.Out, "coverage-report.md"))}; complete={complete}");
        return results.Any(result => result.Status != "pass") || (options.RequireComplete && !complete) ? 1 : 0;
    }

    private static CoverageResult RunFixture(Args options, AnalyzerHost analyzer, CoverageFixture fixture)
    {
        var result = new CoverageResult
        {
            Id = fixture.Id, Fixture = fixture, Verdict = analyzer.Analyze(fixture.Sql),
            ParsedTypes = CoverageCorpus.StatementTypes(fixture.Sql).Order().ToArray()
        };
        CoverageDatabase? database = null;
        try
        {
            database = new CoverageDatabase(options.Server, options.Timeout);
            result.Database = database.DatabaseName;
            result.ServerVersion = database.ServerVersion;
            result.Stage = "setup";
            database.Execute(fixture.SetupSql);
            if (Convert.ToInt32(database.Scalar("SELECT @@TRANCOUNT;")) != 0)
                throw new InvalidDataException("Setup left an ambient transaction.");
            var before = database.Snapshot();
            result.BeforeHash = CoverageDatabase.Hash(before);
            result.Stage = "execute";
            database.Execute(fixture.Sql);
            result.Stage = "verify";
            result.ActualScalar = CoverageCorpus.Scalar(database.Scalar(fixture.VerifySql));
            result.TransactionCount = Convert.ToInt32(database.Scalar("SELECT @@TRANCOUNT;"));
            var visible = database.Snapshot();
            result.VisibleHash = CoverageDatabase.Hash(visible);
            result.VisibleChanged = !before.SequenceEqual(visible);
            result.Difference = before.Except(visible).Select(line => "- " + line).Concat(visible.Except(before).Select(line => "+ " + line)).Take(12).ToArray();
            result.Stage = "persisted-observation";
            var persisted = database.PersistedSnapshot();
            result.PersistedHash = CoverageDatabase.Hash(persisted);
            result.PersistedChanged = !before.SequenceEqual(persisted);
            var errors = new List<string>();
            bool expectedChange = fixture.ExpectedPermanentEffect != "none";
            if (result.Verdict.Overall != fixture.ExpectedVerdict)
                errors.Add($"Expected verdict {fixture.ExpectedVerdict}, got {result.Verdict.Overall}");
            if (result.Verdict.Error != null) errors.Add(result.Verdict.Error);
            if (result.ActualScalar != fixture.ExpectedVerifyScalar.GetRawText())
                errors.Add($"Expected scalar {fixture.ExpectedVerifyScalar}, got {result.ActualScalar}");
            if (result.TransactionCount != fixture.ExpectedTransactionCount)
                errors.Add($"Expected transaction count {fixture.ExpectedTransactionCount}, got {result.TransactionCount}");
            if (result.VisibleChanged != (fixture.ExpectedVisibleChange ?? expectedChange))
                errors.Add($"Unexpected visible database change: {result.VisibleChanged}");
            if (result.PersistedChanged != (fixture.ExpectedPersistedChange ?? expectedChange))
                errors.Add($"Unexpected persisted database change: {result.PersistedChanged}");
            if (result.Verdict.IsGreen && (result.VisibleChanged == true || result.PersistedChanged == true))
                errors.Add("FALSE GREEN: database mutation observed");
            result.Status = errors.Count == 0 ? "pass" : "fail";
            result.Error = errors.Count == 0 ? null : string.Join("; ", errors);
            result.Stage = "complete";
        }
        catch (Exception exception)
        {
            result.Error = exception.GetType().Name + ": " + exception.Message;
        }
        finally
        {
            try { database?.Dispose(); }
            catch (Exception exception)
            {
                result.Status = "execution-error";
                result.Stage = "cleanup";
                result.Error = (result.Error == null ? "" : result.Error + "; ") + "Cleanup failed: " + exception.Message;
            }
        }
        return result;
    }

    private static void Write(Args options, RunIdentity identity, CoverageManifest manifest, CoverageFixture[] fixtures, List<CoverageResult> results)
    {
        var matrix = manifest.Entries.Select(entry => new
        {
            entry.StatementType, DeclaredStatus = entry.Status, entry.Reason, entry.FixtureIds,
            Status = entry.FixtureIds.Length == 0 ? entry.Status :
                entry.FixtureIds.Any(id => results.Any(result => result.Id == id && result.Status == "execution-error")) ? "execution-error" :
                entry.FixtureIds.Any(id => results.Any(result => result.Id == id && result.Status == "fail")) ? "fail" :
                entry.FixtureIds.All(id => results.Any(result => result.Id == id && result.Status == "pass")) ? "pass" : "not-run"
        }).ToArray();
        var directory = Directory.CreateDirectory(options.Out).FullName;
        var metadata = new
        {
            RunUtc = DateTimeOffset.UtcNow, identity.StartedUtc, identity.AnalyzerSha256, identity.HarnessSha256, identity.ManifestSha256, identity.FixturesSha256,
            manifest.ScriptDomVersion, PlannedFixtures = fixtures.Length, ExecutedFixtures = results.Count,
            Complete = matrix.All(entry => entry.Status == "pass"), Matrix = matrix, Results = results
        };
        var jsonPath = Path.Combine(directory, "coverage-results.json");
        File.WriteAllText(jsonPath + ".tmp", JsonSerializer.Serialize(metadata, CoverageCorpus.Json));
        File.Move(jsonPath + ".tmp", jsonPath, true);
        var text = new StringBuilder("# Strict SQL Server coverage evidence\n\n");
        text.AppendLine($"- Run (UTC): {metadata.RunUtc:O}");
        text.AppendLine($"- Analyzer SHA-256: {metadata.AnalyzerSha256}");
        text.AppendLine($"- ScriptDom package: {manifest.ScriptDomVersion}; inventory: {matrix.Length} concrete kinds.");
        text.AppendLine($"- Fixtures: {results.Count}/{fixtures.Length}; " + string.Join(", ", results.GroupBy(result => result.Status).Select(group => $"{group.Key}={group.Count()}")));
        text.AppendLine("- Kind statuses: " + string.Join(", ", matrix.GroupBy(entry => entry.Status).Select(group => $"{group.Key}={group.Count()}")));
        text.AppendLine("- Every fixture uses a fresh uniquely named database and a database-scoped user without a login. Cleanup drops only that database. Existing SqlGuard_Prod/SqlGuard_Dev and SSMS settings are untouched.");
        text.AppendLine("- Only fixture SQL is analyzed. Setup is excluded. Exact table rows and selected catalog state are compared after setup, before closing the session, and after session-close rollback. Errors are not passes.");
        text.AppendLine("- This is representative kind/fixture coverage, NOT every clause combination, execution path, external side effect, SSMS event dispatch, or physical storage change. Body AST nodes alone do not earn coverage. A zero exit code means selected fixtures passed, not complete coverage; use --require-complete to enforce all kinds.\n");
        text.AppendLine("## Fixtures\n\n| ID | Verdict | Visible change | Persisted change | Status |\n|---|---|---|---|---|");
        foreach (var result in results)
            text.AppendLine($"| {result.Id} | {result.Verdict.Overall} | {result.VisibleChanged} | {result.PersistedChanged} | {result.Status} |");
        foreach (var result in results.Where(result => result.Error != null))
            text.AppendLine($"\n### {result.Id} ({result.Stage})\n\n{result.Error}");
        text.AppendLine("\n## Complete inventory\n\n| Kind | Status | Fixtures / reason |\n|---|---|---|");
        foreach (var entry in matrix)
            text.AppendLine($"| {entry.StatementType} | {entry.Status} | {string.Join(", ", entry.FixtureIds)} {entry.Reason} |");
        File.WriteAllText(Path.Combine(directory, "coverage-report.md"), text.ToString());
    }
}
