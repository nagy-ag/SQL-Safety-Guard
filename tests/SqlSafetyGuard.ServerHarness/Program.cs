using System.Text;
using System.Text.Json;
using SqlSafetyGuard.ServerHarness;

// Real-server check of the analyzer: every case is analyzed, then executed on a sandbox as a low-privilege login, and
// the sandbox fingerprint tells whether anything permanent changed. A green verdict on a case that changed data is a
// false green. See README.md in this folder.
//
//   dotnet run --project tests/SqlSafetyGuard.ServerHarness -- --legacy-discovery --server <disposable instance>
//       --analyzer <SqlSafetyGuard.Analyzer.dll>
//       --tests <dir with *.cs> [--tests <dir>...] --samples <dir with *.sql> [--sql <dir>] [--server .\SQLEXPRESS]
//       [--out <dir>] [--limit N] [--filter text] [--timeout 10] [--keep]

var options = Args.Parse(args);
if (options.TestIndex != null)
{
    TestSourceIndex.Write(options.TestIndex, options.Out);
    return 0;
}
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
