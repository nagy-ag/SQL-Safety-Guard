using System.Text;
using System.Text.Json;
using SqlSafetyGuard.ServerHarness;

// Real-server check of the analyzer: every case is analyzed, then executed on a sandbox as a low-privilege login, and
// the sandbox fingerprint tells whether anything permanent changed. A green verdict on a case that changed data is a
// false green. See README.md in this folder.
//
//   dotnet run --project tests/SqlSafetyGuard.ServerHarness -- --analyzer <SqlSafetyGuard.Analyzer.dll>
//       --tests <dir with *.cs> [--tests <dir>...] --samples <dir with *.sql> [--sql <dir>] [--server .\SQLEXPRESS]
//       [--out <dir>] [--limit N] [--filter text] [--timeout 10] [--keep]

var options = Args.Parse(args);
using var harnessLease = new Mutex(false, "Local\\SqlSafetyGuard.ServerHarness");
if (!options.ValidateCoverage)
{
    try
    {
        if (!harnessLease.WaitOne(0)) throw new InvalidOperationException("Another server harness owns the server lease.");
    }
    catch (AbandonedMutexException) { }
}
var outDir = Directory.CreateDirectory(options.Out).FullName;
var analyzer = new AnalyzerHost(options.Analyzer);
Console.WriteLine($"Analyzer: {analyzer.AssemblyPath}");
if (options.Campaign != null)
    return CampaignRunner.Run(options, analyzer);
if (options.IsolatedLifecycle)
    return LifecycleCoverageRunner.Run(options, analyzer);
if (options.IsolatedCoverage != null)
    return IsolatedCoverageRunner.Run(options, analyzer);
if (options.Coverage != null)
    return CoverageRunner.Run(options, analyzer);

var cases = new List<Case>();
foreach (var dir in options.Tests)
    cases.AddRange(Corpus.FromCSharp(Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
        .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))));
foreach (var dir in options.Samples.Concat(options.Sql))
    cases.AddRange(Corpus.FromSqlFiles(Directory.EnumerateFiles(dir, "*.sql", SearchOption.AllDirectories)));
cases.AddRange(Generator.All());
cases.AddRange(Variants.All());
cases.Add(new Case("gen:red-crossdb-dev", "generated", "UPDATE SqlGuard_Dev.dbo.Orders SET Qty = Qty + 1 WHERE Id = 1;"));
cases.Add(new Case("gen:green-crossdb-read", "generated", "SELECT COUNT(*) FROM SqlGuard_Dev.dbo.Orders AS d JOIN SqlGuard_Prod.dbo.Orders AS p ON p.Id = d.Id;"));
cases = Corpus.Dedupe(cases);
if (options.Filter != null)
    cases = cases.Where(c => c.Id.Contains(options.Filter, StringComparison.OrdinalIgnoreCase) || c.Sql.Contains(options.Filter, StringComparison.OrdinalIgnoreCase)).ToList();
if (options.Limit > 0)
    cases = cases.Take(options.Limit).ToList();
Console.WriteLine($"Cases: {cases.Count} ({cases.Count(c => c.Source == "generated")} generated)");

var schema = new SchemaDiscovery();
foreach (var c in cases)
{
    try { schema.Add(c.Sql); } catch { /* discovery is best effort */ }
}

using var sandbox = new Sandbox(options.Server, schema);
Console.WriteLine("Setting up sandbox (SqlGuard_Prod, SqlGuard_Dev)...");
sandbox.Setup();
Console.WriteLine($"Sandbox ready: {sandbox.TableCount} tables per database, {schema.Columns.Count} shared columns, {schema.Sequences.Count} sequences.");

var runner = new Runner(sandbox, schema, options.Timeout);
var results = new List<Result>();
int index = 0;
foreach (var c in cases)
{
    index++;
    var verdict = analyzer.Analyze(c.Sql);
    var exec = runner.Run(c.Sql);
    var fingerprint = sandbox.Fingerprint();
    bool changed = !sandbox.MatchesBaseline(fingerprint);
    var diff = changed ? sandbox.Diff(fingerprint) : new List<string>();
    if (changed)
        sandbox.Revert(fingerprint);
    sandbox.AssertServerUnchanged(c.Id);

    var result = new Result(c, verdict, exec, changed || exec.UncommittedLogRecords > 0, diff);
    results.Add(result);
    var flag = result.Problem != null ? $"  <-- {result.Problem}" : "";
    Console.WriteLine($"[{index}/{cases.Count}] {c.Id,-48} {verdict.Overall,-12} wrote={(result.Wrote ? "YES" : "no ")} {verdict.Milliseconds,7:F0} ms{flag}");
}

if (!options.Keep)
    sandbox.Teardown();

Report.Write(outDir, analyzer.AssemblyPath, results);
Console.WriteLine($"Report: {Path.Combine(outDir, "report.md")} (sandbox rebuilds: {sandbox.Rebuilds})");
int problems = results.Count(r => r.Problem != null);
Console.WriteLine(problems == 0 ? "No false greens, crashes or failed expectations." : $"{problems} problem(s): see the report.");
return problems == 0 ? 0 : 1;

internal sealed record Result(Case Case, Verdict Verdict, Execution Exec, bool Wrote, List<string> Diff)
{
    /// <summary>The reason this case is a problem, or null.</summary>
    public string? Problem
    {
        get
        {
            if (Verdict.Overall == "CRASH")
                return "ANALYZER CRASH";
            if (Verdict.IsGreen && Wrote)
                return "FALSE GREEN";
            if (Generator.ExpectGreen(Case) && (!Verdict.IsGreen || Wrote || Exec.FailedBatches > 0))
                return $"EXPECTED GREEN, no write, no errors (got {Verdict.Overall}, wrote={Wrote}, failed batches={Exec.FailedBatches})";
            if (Generator.ExpectWrite(Case) && (Verdict.IsGreen || !Wrote))
                return $"EXPECTED RED + WRITE (got {Verdict.Overall}, wrote={Wrote}, failed batches={Exec.FailedBatches})";
            return null;
        }
    }
}

internal static class Report
{
    public static void Write(string dir, string analyzerPath, List<Result> results)
    {
        var md = new StringBuilder();
        md.AppendLine("# SQL Safety Guard: real-server harness report");
        md.AppendLine();
        md.AppendLine($"- Analyzer: `{analyzerPath}`");
        md.AppendLine($"- Run: {DateTime.Now:yyyy-MM-dd HH:mm}");
        md.AppendLine($"- Cases: {results.Count} · executed: {results.Count(r => r.Exec.Ran)} · executed with no errors: {results.Count(r => r.Exec.Ran && r.Exec.FailedBatches == 0)} · not run (client-side SQLCMD): {results.Count(r => !r.Exec.Ran)}");
        md.AppendLine($"- Changed permanent data/schema/state: {results.Count(r => r.Wrote)} · of those analyzed green: **{results.Count(r => r.Wrote && r.Verdict.IsGreen)}**");
        md.AppendLine($"- Verdicts: " + string.Join(", ", results.GroupBy(r => r.Verdict.Overall).OrderBy(g => g.Key).Select(g => $"{g.Key} {g.Count()}")));
        md.AppendLine();

        var problems = results.Where(r => r.Problem != null).ToList();
        md.AppendLine($"## Problems ({problems.Count})");
        md.AppendLine();
        foreach (var r in problems)
            AppendCase(md, r, r.Problem!);
        if (problems.Count == 0)
            md.AppendLine("None.");
        md.AppendLine();

        var generated = results.Where(r => r.Case.Source == "generated").ToList();
        md.AppendLine("## Generated large scripts");
        md.AppendLine();
        md.AppendLine("| Case | Lines | KB | Verdict | Wrote | Failed batches | Analyze ms |");
        md.AppendLine("|---|---:|---:|---|---|---:|---:|");
        foreach (var r in generated)
            md.AppendLine($"| {r.Case.Id} | {r.Case.Sql.Count(ch => ch == '\n')} | {r.Case.Sql.Length / 1024} | {r.Verdict.Overall} | {(r.Wrote ? "yes" : "no")} | {r.Exec.FailedBatches} | {r.Verdict.Milliseconds:F0} |");
        md.AppendLine();

        var noisy = results.Where(r => r.Exec.Ran && r.Exec.FailedBatches == 0 && !r.Wrote && !r.Verdict.IsGreen).ToList();
        md.AppendLine($"## Not green, ran cleanly, changed nothing ({noisy.Count}) — review for warning fatigue");
        md.AppendLine();
        md.AppendLine("Many are correct (e.g. BEGIN TRAN ... ROLLBACK, procedures, statements the login may not run); listed so false reds can be spotted.");
        md.AppendLine();
        foreach (var r in noisy.Take(80))
            md.AppendLine($"- `{r.Case.Id}` {r.Verdict.Overall}: `{OneLine(r.Case.Sql, 140)}`");
        md.AppendLine();

        md.AppendLine("## Slowest analyses");
        md.AppendLine();
        foreach (var r in results.OrderByDescending(r => r.Verdict.Milliseconds).Take(10))
            md.AppendLine($"- {r.Verdict.Milliseconds:F0} ms · {r.Case.Sql.Length / 1024} KB · `{r.Case.Id}`");

        File.WriteAllText(Path.Combine(dir, "report.md"), md.ToString());
        File.WriteAllText(Path.Combine(dir, "results.json"), JsonSerializer.Serialize(results.Select(r => new
        {
            r.Case.Id, r.Case.Source, Sql = r.Case.Sql.Length > 4000 ? r.Case.Sql[..4000] + "…" : r.Case.Sql,
            r.Verdict.Overall, r.Verdict.IsGreen, r.Verdict.Milliseconds, r.Verdict.Findings, r.Verdict.Error,
            r.Exec.Ran, r.Exec.SkipReason, r.Exec.Batches, r.Exec.FailedBatches, r.Exec.Errors, r.Exec.UncommittedLogRecords, r.Exec.TimedOut,
            r.Wrote, r.Diff, r.Problem,
        }), new JsonSerializerOptions { WriteIndented = true }));
    }

    private static void AppendCase(StringBuilder md, Result r, string title)
    {
        md.AppendLine($"### {title}: `{r.Case.Id}`");
        md.AppendLine();
        md.AppendLine("```sql");
        md.AppendLine(r.Case.Sql.Length > 1500 ? r.Case.Sql[..1500] + "\n-- … (truncated)" : r.Case.Sql.TrimEnd());
        md.AppendLine("```");
        md.AppendLine($"Analyzer: **{r.Verdict.Overall}** · wrote: **{r.Wrote}** · uncommitted log records: {r.Exec.UncommittedLogRecords} · failed batches: {r.Exec.FailedBatches}");
        if (r.Verdict.Error != null)
            md.AppendLine($"\nCrash: `{OneLine(r.Verdict.Error, 300)}`");
        foreach (var f in r.Verdict.Findings.Take(8))
            md.AppendLine($"- finding: {f}");
        foreach (var d in r.Diff)
            md.AppendLine($"- changed: `{d}`");
        foreach (var e in r.Exec.Errors)
            md.AppendLine($"- error: {e}");
        md.AppendLine();
    }

    private static string OneLine(string s, int max)
    {
        var one = string.Join(" ", s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).Replace("`", "'");
        return one.Length > max ? one[..max] + "…" : one;
    }
}

internal sealed class Args
{
    public string Analyzer { get; set; } = "";
    public List<string> Tests { get; } = new();
    public List<string> Samples { get; } = new();
    public List<string> Sql { get; } = new();
    public string Server { get; set; } = @".\SQLEXPRESS";
    public string Out { get; set; } = "harness-out";
    public int Limit { get; set; }
    public string? Filter { get; set; }
    public int Timeout { get; set; } = 10;
    public bool Keep { get; set; }
    public string? Coverage { get; set; }
    public bool ValidateCoverage { get; set; }
    public bool RequireComplete { get; set; }
    public string? IsolatedCoverage { get; set; }
    public bool IsolatedLifecycle { get; set; }
    public string? Campaign { get; set; }
    public int Workers { get; set; } = Math.Clamp(Environment.ProcessorCount / 2, 1, 4);
    public int BudgetMinutes { get; set; } = 480;
    public int ShrinkAttempts { get; set; } = 24;
    public bool CampaignIsolated { get; set; }
    public bool PrepareWorld { get; set; }
    public bool CampaignOffline { get; set; }
    public bool CampaignValidateOnly { get; set; }
    public string? CampaignPrincipal { get; set; }

    public static Args Parse(string[] args)
    {
        var a = new Args();
        for (int i = 0; i < args.Length; i++)
        {
            string Next() => args[++i];
            switch (args[i])
            {
                case "--analyzer": a.Analyzer = Next(); break;
                case "--tests": a.Tests.Add(Next()); break;
                case "--samples": a.Samples.Add(Next()); break;
                case "--sql": a.Sql.Add(Next()); break;
                case "--server": a.Server = Next(); break;
                case "--out": a.Out = Next(); break;
                case "--limit": a.Limit = int.Parse(Next()); break;
                case "--filter": a.Filter = Next(); break;
                case "--timeout": a.Timeout = int.Parse(Next()); break;
                case "--keep": a.Keep = true; break;
                case "--coverage": a.Coverage = Next(); break;
                case "--isolated-coverage": a.IsolatedCoverage = Next(); break;
                case "--isolated-lifecycle": a.IsolatedLifecycle = true; break;
                case "--validate-coverage": a.ValidateCoverage = true; break;
                case "--require-complete": a.RequireComplete = true; break;
                case "--campaign": a.Campaign = Next(); break;
                case "--workers": a.Workers = int.Parse(Next()); break;
                case "--budget-minutes": a.BudgetMinutes = int.Parse(Next()); break;
                case "--shrink-attempts": a.ShrinkAttempts = int.Parse(Next()); break;
                case "--campaign-isolated": a.CampaignIsolated = true; break;
                case "--prepare-world": a.PrepareWorld = true; break;
                case "--campaign-offline": a.CampaignOffline = true; break;
                case "--campaign-validate": a.CampaignValidateOnly = true; break;
                case "--campaign-principal": a.CampaignPrincipal = Next(); break;
                default: throw new ArgumentException($"Unknown argument {args[i]}");
            }
        }

        var coverageModes = (a.Coverage != null ? 1 : 0) + (a.IsolatedCoverage != null ? 1 : 0) + (a.IsolatedLifecycle ? 1 : 0);
        if (a.Campaign != null && (coverageModes != 0 || a.Keep || a.Tests.Count + a.Sql.Count + a.Samples.Count != 0 || a.ValidateCoverage || a.RequireComplete))
            throw new ArgumentException("Campaign mode cannot combine coverage or discovery modes.");
        if (a.Workers < 1 || a.Workers > 16 || a.BudgetMinutes < 0 || a.Timeout < 1 || a.ShrinkAttempts < 0 || a.Limit < 0)
            throw new ArgumentException("Invalid campaign bounds.");
        if (a.CampaignPrincipal != null && a.CampaignPrincipal != "admin") throw new ArgumentException("Only explicit admin replay is supported.");
        if ((a.CampaignOffline || a.CampaignValidateOnly || a.CampaignPrincipal != null) && a.Campaign == null) throw new ArgumentException("Campaign switches require --campaign.");
        if (a.CampaignIsolated && a.Campaign == null) throw new ArgumentException("--campaign-isolated requires --campaign.");
        if (a.PrepareWorld && a.Campaign == null) throw new ArgumentException("--prepare-world requires --campaign.");
        if (coverageModes > 1 || (coverageModes == 0 && (a.ValidateCoverage || a.RequireComplete)))
            throw new ArgumentException("Select exactly one coverage mode when using coverage-only options.");
        if (a.IsolatedCoverage != null && (a.Keep || a.Tests.Count + a.Sql.Count + a.Samples.Count != 0))
            throw new ArgumentException("Isolated coverage cannot combine discovery inputs or --keep.");
        if (string.IsNullOrEmpty(a.Analyzer))
            throw new ArgumentException("--analyzer <path to SqlSafetyGuard.Analyzer.dll> is required");
        return a;
    }
}
