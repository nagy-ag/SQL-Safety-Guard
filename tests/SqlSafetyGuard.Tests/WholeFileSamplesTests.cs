using System.Text.RegularExpressions;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>Analyzes the sample files as a whole, the way F5 with nothing selected does.</summary>
public class WholeFileSamplesTests
{
    private static string Sample(string name)
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            var path = Path.Combine(dir.FullName, "samples", name);
            if (File.Exists(path))
                return File.ReadAllText(path);
        }

        throw new FileNotFoundException(name);
    }

    [Fact]
    public void TryThese_EveryExampleIsAnalyzed_OnlyExample12FailsToParse()
    {
        var text = Sample("01-try-these.sql");
        var lines = text.Split('\n');
        var result = SqlSafetyAnalyzer.Analyze(text);

        // Example headers and their line ranges.
        var headers = lines.Select((line, i) => (Match: Regex.Match(line, @"^-- #(\d+)"), Line: i + 1))
            .Where(x => x.Match.Success)
            .Select(x => (Number: int.Parse(x.Match.Groups[1].Value), x.Line))
            .ToList();
        Assert.Equal(36, headers.Count);

        for (int i = 0; i < headers.Count; i++)
        {
            int from = headers[i].Line, to = i + 1 < headers.Count ? headers[i + 1].Line - 1 : lines.Length;
            Assert.True(result.Findings.Any(f => f.Line >= from && f.Line <= to),
                $"example #{headers[i].Number} (lines {from}-{to}) has no findings in the whole-file analysis");
        }

        var example12 = headers.Single(h => h.Number == 12).Line;
        var example13 = headers.Single(h => h.Number == 13).Line;
        var error = Assert.Single(result.ParseErrors);
        Assert.InRange(error.Line, example12, example13 - 1);
        Assert.InRange(Assert.Single(result.SkippedRegions).StartLine, example12, example13 - 1);

        // Only example 12's own batch fails, so everything else runs.
        Assert.All(result.Findings.Where(f => f.Line < example12 || f.Line >= example13), f => Assert.True(f.WillRun, f.ToString()));
    }

    [Fact]
    public void BrokenBatches_WithSqlCmdModeOff_Batch5FailsWithHint()
    {
        var text = Sample("02-broken-batches.sql");
        var lines = text.Split('\n');
        int LineOf(string snippet) => Array.FindIndex(lines, l => l.Contains(snippet, StringComparison.Ordinal)) + 1;

        var result = SqlSafetyAnalyzer.Analyze(text, new AnalyzerOptions(sqlCmdMode: false));

        var hint = Assert.Single(result.Findings, f => f.Action == "SQLCMD MODE OFF");
        Assert.Equal(LineOf(":setvar TargetTable"), hint.Line);
        Assert.Contains(result.ParseErrors, e => e.Line == LineOf(":setvar TargetTable"));
        Assert.DoesNotContain(result.Findings, f => f.Line == LineOf("UPDATE $(TargetTable)") && f.WillRun && !f.IsDiagnostic);
        // Batch 3 is untouched by the mode.
        Assert.True(result.Findings.Single(f => f.Line == LineOf("SELECT * FROM dbo.Customers")).WillRun);
    }

    [Fact]
    public void BrokenBatches_ReportsEachBatchHonestly()
    {
        var text = Sample("02-broken-batches.sql");
        var lines = text.Split('\n');
        int LineOf(string snippet) => Array.FindIndex(lines, l => l.Contains(snippet, StringComparison.Ordinal)) + 1;

        var result = SqlSafetyAnalyzer.Analyze(text);

        // Batch 2: syntax error; its statements are listed but won't run.
        Assert.Contains(result.ParseErrors, e => e.Line == LineOf("FROM dbo.Orders WHERE;"));
        var update = result.Findings.Single(f => f.Action == "UPDATE" && f.Line == LineOf("SET Qty = Qty + 1"));
        Assert.False(update.WillRun);
        var deleteAll = result.Findings.Single(f => f.Action == "DELETE" && f.Line == LineOf("DELETE FROM dbo.AuditLog;"));
        Assert.False(deleteAll.WillRun);
        Assert.True(deleteAll.AffectsAllRows);

        // Batch 3 runs.
        Assert.True(result.Findings.Single(f => f.Line == LineOf("SELECT * FROM dbo.Customers")).WillRun);

        // Batch 4: misplaced CREATE PROCEDURE; its body is not reported as a DELETE that runs.
        var create = result.Findings.Single(f => f.Action == "CREATE PROCEDURE");
        Assert.Equal("dbo.usp_Archive", create.Target);
        Assert.Contains("add GO before it", create.Note);
        Assert.DoesNotContain(result.Findings, f => f.Line == LineOf("WHERE Id < 0"));
        Assert.False(result.Findings.Single(f => f.Line == LineOf("SET X = 2")).WillRun);

        // Batch 5: SQLCMD.
        var sqlcmdUpdate = result.Findings.Single(f => f.Line == LineOf("UPDATE $(TargetTable)"));
        Assert.Equal("dbo.Orders", sqlcmdUpdate.ResolvedTarget);
        Assert.True(sqlcmdUpdate.WillRun);
        Assert.Contains(result.Findings, f => f.Action == ":r" && f.Verdict == Verdict.Unknown);

        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.ParseErrors, e => e.Line == LineOf("CREATE PROCEDURE dbo.usp_Archive"));
        Assert.Equal(new[] { LineOf("FROM dbo.Orders WHERE;"), LineOf("CREATE PROCEDURE dbo.usp_Archive") },
            result.ParseErrors.Select(e => e.Line).Distinct());
    }
}
