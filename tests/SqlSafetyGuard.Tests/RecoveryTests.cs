using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>Batches with syntax errors: statements around the error are still analyzed and labelled "won't run now".</summary>
public class RecoveryTests
{
    private static AnalysisResult Analyze(string sql) => SqlSafetyAnalyzer.Analyze(sql);

    private static int LineOf(string sql, string text) => sql.Substring(0, sql.IndexOf(text, StringComparison.Ordinal)).Split('\n').Length;

    [Fact]
    public void ErrorInTheMiddle_OtherStatementsAreAnalyzedAndBlocked()
    {
        var sql = "SELECT 1;\nUPDATE dbo.Orders SET Qty = 1 WHERE Id = 1;\nSELECT * FROM dbo.Orders WHERE;\nDELETE FROM dbo.AuditLog WHERE Id = 5;\n";
        var result = Analyze(sql);

        Assert.Equal(Verdict.RealWrite, result.Overall);
        var error = Assert.Single(result.ParseErrors);
        Assert.Equal(3, error.Line);

        var update = Assert.Single(result.Findings, f => f.Action == "UPDATE");
        var delete = Assert.Single(result.Findings, f => f.Action == "DELETE");
        Assert.Equal(2, update.Line);
        Assert.Equal(4, delete.Line);
        Assert.False(update.WillRun);
        Assert.False(delete.WillRun);
        Assert.Equal(3, update.BlockingErrorLine);
        Assert.Contains("won't run now: this batch has a syntax error at line 3", update.Note);

        var region = Assert.Single(result.SkippedRegions);
        Assert.Equal(3, region.StartLine);
        Assert.Equal(3, region.EndLine);
        Assert.Contains(result.Findings, f => f.Action == "NOT ANALYZED" && f.Target == "line 3");
    }

    [Fact]
    public void OnlyTheBrokenBatchIsBlocked()
    {
        var sql = "UPDATE dbo.A SET x = 1 WHERE y = 2;\nGO\nSELECT * FROM;\nUPDATE dbo.B SET x = 1 WHERE y = 2;\nGO\nUPDATE dbo.C SET x = 1 WHERE y = 2;\n";
        var result = Analyze(sql);

        Assert.True(result.Findings.Single(f => f.Target == "dbo.A").WillRun);
        Assert.False(result.Findings.Single(f => f.Target == "dbo.B").WillRun);
        Assert.True(result.Findings.Single(f => f.Target == "dbo.C").WillRun);
        Assert.Equal("2 real writes, 1 syntax error \u00B7 1 more in a batch that will fail", result.Summary);
    }

    [Fact]
    public void IncompleteStatement_ReportedAtNextStatement_BlanksTheIncompleteOne()
    {
        // The parser reports "Incorrect syntax near 'DELETE'", but the broken statement is the SELECT before it.
        var sql = "SELECT * FROM dbo.Orders WHERE\nDELETE FROM dbo.Log WHERE Id = 1;\n";
        var result = Analyze(sql);

        var delete = Assert.Single(result.Findings, f => f.Action == "DELETE");
        Assert.Equal(2, delete.Line);
        Assert.False(delete.WillRun);
        Assert.Equal(1, Assert.Single(result.SkippedRegions).StartLine);
    }

    [Fact]
    public void ErrorInsideIfBlock_KeepsTheRest()
    {
        var sql = "IF 1 = 1\nBEGIN\n    SELECT * FROM WHERE;\nEND\nELSE\n    DELETE FROM dbo.Orders WHERE Id = 1;\nTRUNCATE TABLE dbo.Log;\n";
        var result = Analyze(sql);

        Assert.Contains(result.Findings, f => f.Action == "DELETE" && f.Line == 6 && !f.WillRun);
        Assert.Contains(result.Findings, f => f.Action == "TRUNCATE TABLE" && f.Line == 7 && !f.WillRun);
        Assert.DoesNotContain(result.Findings, f => f.Action == "PRINT");
    }

    [Fact]
    public void SeveralErrors_AllReported_RestAnalyzed()
    {
        var sql = "SELECT * FROM WHERE;\nUPDATE dbo.A SET x = 1 WHERE Id = 1;\nSELECT FROM;\nUPDATE dbo.B SET x = 1 WHERE Id = 1;\n";
        var result = Analyze(sql);

        Assert.Equal(2, result.SkippedRegions.Count);
        Assert.Equal(new[] { 1, 3 }, result.SkippedRegions.Select(r => r.StartLine));
        Assert.Equal(2, result.Findings.Count(f => f.Action == "UPDATE" && !f.WillRun));
    }

    [Fact]
    public void MisplacedCreateProcedure_IsReportedAndSwallowsTheRestOfTheBatch()
    {
        var sql = "UPDATE dbo.A SET x = 1 WHERE Id = 1;\nCREATE OR ALTER PROCEDURE dbo.usp_Report AS\n    DELETE FROM dbo.Orders;\nUPDATE dbo.B SET x = 1 WHERE Id = 1;\nGO\nSELECT 1;\n";
        var result = Analyze(sql);

        var create = Assert.Single(result.Findings, f => f.Action == "CREATE OR ALTER PROCEDURE");
        Assert.Equal("dbo.usp_Report", create.Target);
        Assert.Equal(Verdict.RealWrite, create.Verdict);
        Assert.Contains("must be the first statement in its batch: add GO before it", create.Note);

        // The DELETE and the second UPDATE would be the procedure's body: they are not reported as statements that run.
        Assert.DoesNotContain(result.Findings, f => f.Action == "DELETE");
        Assert.DoesNotContain(result.Findings, f => f.Target == "dbo.B");
        Assert.DoesNotContain(result.Findings, f => f.Action.StartsWith("possible", StringComparison.Ordinal));

        Assert.False(result.Findings.Single(f => f.Target == "dbo.A").WillRun);
        Assert.True(result.Findings.Single(f => f.Action == "SELECT").WillRun);
        Assert.Equal("lines 2\u20134", Assert.Single(result.SkippedRegions).Lines);
    }

    [Fact]
    public void ProcedureAtBatchStart_StillWorksAsBefore()
    {
        var result = Analyze("UPDATE dbo.A SET x = 1 WHERE Id = 1;\nGO\nCREATE PROCEDURE dbo.p AS DELETE FROM dbo.Orders;\nGO\n");
        Assert.Empty(result.ParseErrors);
        Assert.Contains(result.Findings, f => f.Action == "CREATE PROCEDURE" && f.WillRun);
    }

    [Fact]
    public void KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings()
    {
        // ")" closes nothing, so the whole statement can't be parsed.
        var sql = "UPDATE dbo.Orders SET Qty = 1 WHERE Id = 1) -- DELETE in a comment\n    AND Note = 'TRUNCATE';\nSELECT 1;\n";
        var result = Analyze(sql);

        Assert.NotEmpty(result.SkippedRegions);
        var possible = result.Findings.Where(f => f.Action.StartsWith("possible ", StringComparison.Ordinal)).ToList();
        Assert.Contains(possible, f => f.Action == "possible UPDATE" && f.Verdict == Verdict.Unknown && !f.WillRun);
        Assert.DoesNotContain(possible, f => f.Action == "possible DELETE");
        Assert.DoesNotContain(possible, f => f.Action == "possible TRUNCATE");
    }

    [Fact]
    public void RecoveredFindings_KeepExactPositions()
    {
        var sql = "SELECT * FROM WHERE;\n   UPDATE dbo.T SET x = 1 WHERE y = 2;\n";
        var update = Assert.Single(Analyze(sql).Findings, f => f.Action == "UPDATE");
        Assert.Equal(2, update.Line);
        Assert.Equal(4, update.Column);
        Assert.Equal(sql.IndexOf("UPDATE", StringComparison.Ordinal), update.StartOffset);
    }

    [Fact]
    public void BlockedRedStillMakesOverallRed()
    {
        var result = Analyze("DELETE FROM dbo.Orders WHERE Id = 1;\nSELECT * FROM WHERE;\n");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Equal("1 syntax error \u00B7 1 in a batch that will fail", result.Summary);
        Assert.Equal(1, result.BlockedWarningCount);
    }

    [Fact]
    public void UnrecognizedCharacter_DoesNotHideWhatFollows()
    {
        // The tokenizer rejects "\" outside strings; the GO, the DELETE after it and the next batch must still be seen.
        var sql = "SELECT 1;\nUPDATE dbo.A SET x = 1 WHERE y = 2 \\ oops\nDELETE FROM dbo.B WHERE Id = 1;\nGO\nUPDATE dbo.C SET x = 1 WHERE y = 2;\n";
        var result = Analyze(sql);

        Assert.True(result.Findings.Single(f => f.Target == "dbo.C").WillRun);
        Assert.Contains(result.Findings, f => (f.Action == "DELETE" || f.Action == "possible DELETE") && f.Line == 3 && !f.WillRun);
    }

    [Fact]
    public void GiveUp_WholeBatchIsNotAnalyzed_AndScanned()
    {
        // Unterminated comment: nothing after it can be tokenized as code.
        var sql = "UPDATE dbo.A SET x = 1 WHERE Id = 1;\n/* never closed\nDELETE FROM dbo.B;\n";
        var result = Analyze(sql);
        Assert.True(result.Overall >= Verdict.Unknown);
        Assert.NotEmpty(result.ParseErrors);
        Assert.NotEmpty(result.SkippedRegions);
    }
}
