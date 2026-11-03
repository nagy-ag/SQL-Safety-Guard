using System.Text;
using System.Text.Json;
using SqlSafetyGuard.ServerHarness;

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
