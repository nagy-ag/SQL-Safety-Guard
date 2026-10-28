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

public class NoWhereTests
{
    private static Finding Single(string sql, string action) =>
        Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings, f => f.Action == action);

    [Fact]
    public void DeleteWithoutWhere_AffectsEveryRow()
    {
        var f = Single("DELETE FROM dbo.Orders", "DELETE");
        Assert.True(f.AffectsAllRows);
        Assert.Contains("no WHERE: affects every row", f.Note);
    }

    [Fact]
    public void UpdateWithoutWhere_AffectsEveryRow()
    {
        Assert.True(Single("UPDATE dbo.Orders SET Qty = 0", "UPDATE").AffectsAllRows);
    }

    [Fact]
    public void WithWhere_NoNote()
    {
        var f = Single("UPDATE dbo.Orders SET Qty = 0 WHERE Id = 1", "UPDATE");
        Assert.False(f.AffectsAllRows);
        Assert.DoesNotContain("no WHERE", f.Note);
    }

    [Theory]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o JOIN #ids i ON i.Id = o.Id", "UPDATE", "rows limited only by the JOIN")]
    [InlineData("UPDATE o SET Qty = 0 FROM #ids i INNER JOIN dbo.Orders o ON i.Id = o.Id LEFT JOIN dbo.Log l ON l.Id = o.Id", "UPDATE", "rows limited only by the JOIN")]
    [InlineData("UPDATE dbo.Orders SET Qty = 0 FROM dbo.Orders JOIN #ids i ON i.Id = Orders.Id", "UPDATE", "rows limited only by the JOIN")]
    [InlineData("DELETE o FROM dbo.Orders o CROSS APPLY (SELECT TOP 1 Id FROM #ids i WHERE i.Id = o.Id) x", "DELETE", "rows limited only by the JOIN")]
    [InlineData("DELETE TOP (10) FROM dbo.Orders", "DELETE", "limited only by TOP")]
    public void NoWhere_VisiblyLimitedRows_AreNotWholeTarget(string sql, string action, string note)
    {
        var f = Single(sql, action);
        Assert.Equal(Verdict.RealWrite, f.Verdict);
        Assert.False(f.AffectsAllRows);
        Assert.Equal(string.Empty, f.AllRowsSummary);
        Assert.Contains(note, f.Note);
    }

    [Theory]
    [InlineData("WITH c AS (SELECT * FROM dbo.Orders WHERE Qty = 0) DELETE FROM c", "DELETE", "rows limited only by the CTE or derived table")]
    [InlineData("UPDATE TOP (100) PERCENT dbo.Orders SET Qty = 0", "UPDATE", "limited only by TOP PERCENT")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o LEFT JOIN #ids i ON i.Id = o.Id", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o LEFT JOIN (dbo.A a JOIN dbo.B b ON b.Id = a.Id) ON a.Id = o.Id", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o CROSS JOIN #ids i", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE dbo.Orders SET Qty = 0 FROM #a a JOIN #b b ON b.Id = a.Id", "UPDATE", "the FROM has no inner join")]
    public void NoWhere_UnprovenLimits_RemainWholeTarget(string sql, string action, string note)
    {
        var f = Single(sql, action);
        Assert.True(f.AffectsAllRows);
        Assert.Contains(note, f.Note);
    }

    [Theory]
    [InlineData("DELETE FROM dbo.Orders", "No WHERE clause: can affect every row of dbo.Orders")]
    [InlineData("WITH c AS (SELECT * FROM dbo.Orders WHERE Qty = 0) DELETE FROM c", "No WHERE clause: only the CTE or derived table limits which rows of c → dbo.Orders change")]
    [InlineData("TRUNCATE TABLE dbo.Log", "Removes every row of dbo.Log")]
    [InlineData("DROP TABLE dbo.Log", "Drops dbo.Log and all its rows")]
    [InlineData("DROP PROCEDURE dbo.p", "Drops dbo.p")]
    public void AllRowsSummary_DescribesTheActualChange(string sql, string summary)
    {
        var f = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings, x => x.AffectsAllRows);
        Assert.Equal(summary, f.AllRowsSummary);
    }

    [Fact]
    public void LegacyTypedConfirmationIsIgnoredForAllWrites()
    {
        var production = new GuardSettings(ExecutionMode.YellowRed, "Production", true, new string[0], new string[0]);
        Assert.False(production.NeedsTypedConfirmation(SqlSafetyAnalyzer.Analyze(
            "UPDATE o SET Qty = 0 FROM dbo.Orders o JOIN #ids i ON i.Id = o.Id")));
Assert.False(production.NeedsTypedConfirmation(SqlSafetyAnalyzer.Analyze("UPDATE dbo.Orders SET Qty = 0")));
    }

    [Fact]
    public void Truncate_RemovesEveryRow()
    {
        var f = Single("TRUNCATE TABLE dbo.Log", "TRUNCATE TABLE");
        Assert.True(f.AffectsAllRows);
        Assert.Contains("removes every row", f.Note);
    }

    [Fact]
    public void CursorUpdate_HasAWhere()
    {
        Assert.False(Single("UPDATE dbo.Orders SET Qty = 0 WHERE CURRENT OF c", "UPDATE").AffectsAllRows);
    }

    [Fact]
    public void TempTableWithoutWhere_GetsNoteButStaysGreen()
    {
        var result = SqlSafetyAnalyzer.Analyze("DELETE FROM #work");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.True(Assert.Single(result.Findings).AffectsAllRows);
    }
}

public class SqlCmdTests
{
    private const string Script =
        ":setvar TargetTable dbo.Orders\r\n" +
        ":setvar Scratch \"#work\"\r\n" +
        ":on error exit\r\n" +
        "UPDATE $(TargetTable) SET Status = 'checked' WHERE Id = 2;\r\n" +
        "INSERT $(Scratch) VALUES (1);\r\n" +
        ":r .\\more-changes.sql\r\n" +
        "!!dir C:\\\r\n" +
        ":connect OtherServer\r\n" +
        "GO\r\n" +
        "SELECT 1;\r\n";

    [Fact]
    public void SqlCmdScript_ParsesWithoutErrors()
    {
        var result = SqlSafetyAnalyzer.Analyze(Script);
        Assert.Empty(result.ParseErrors);
        Assert.Empty(result.SkippedRegions);
    }

    [Fact]
    public void Variables_AreResolvedForDisplayAndClassification()
    {
        var result = SqlSafetyAnalyzer.Analyze(Script);
        var update = Assert.Single(result.Findings, f => f.Action == "UPDATE");
        Assert.Equal("$(TargetTable)", update.Target);
        Assert.Equal("dbo.Orders", update.ResolvedTarget);
        Assert.Equal(Verdict.RealWrite, update.Verdict);
        Assert.Equal(4, update.Line);

        var insert = Assert.Single(result.Findings, f => f.Action == "INSERT");
        Assert.Equal(Verdict.TempOnly, insert.Verdict);
    }

    [Fact]
    public void Directives_AreReported()
    {
        var result = SqlSafetyAnalyzer.Analyze(Script);
        Assert.Contains(result.Findings, f => f.Action == ":r" && f.Verdict == Verdict.Unknown && f.Target == @".\more-changes.sql");
        Assert.Contains(result.Findings, f => f.Action == "!!" && f.Verdict == Verdict.Unknown && f.Line == 7);
        Assert.Contains(result.Findings, f => f.Action == "TARGET SWITCH" && f.Verdict == Verdict.Unknown);
        Assert.Equal(2, result.Findings.Count(f => f.Action == ":setvar"));
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Fact]
    public void UndefinedVariable_IsTreatedAsPermanentName()
    {
        var result = SqlSafetyAnalyzer.Analyze("DELETE FROM $(Table) WHERE Id = 1;");
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Equal("$(Table)", Assert.Single(result.Findings, finding => finding.Action == "DELETE").Target);
        Assert.Contains(result.Findings, finding => finding.Action == "SQLCMD VARIABLE" && finding.Verdict == Verdict.Unknown);
    }

    private static AnalysisResult WithMode(string sql, bool? mode) => SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(sqlCmdMode: mode));

    [Fact]
    public void ModeOn_SameAsUnknown()
    {
        Assert.Equal(WithMode(Script, null).Summary, WithMode(Script, true).Summary);
        Assert.Empty(WithMode(Script, true).ParseErrors);
    }

    [Fact]
    public void ModeOff_SqlCmdLinesAreSyntaxErrors_WithHint()
    {
        var result = WithMode(Script, false);

        // What the server would get: the first batch (all the SQLCMD lines) fails; the second batch still runs.
        Assert.NotEmpty(result.ParseErrors);
        var hint = Assert.Single(result.Findings, f => f.Action == "SQLCMD MODE OFF");
        Assert.Equal(1, hint.Line);
        Assert.Contains("turn on Query > SQLCMD Mode", hint.Note);
        Assert.True(hint.IsDiagnostic);

        Assert.DoesNotContain(result.Findings, f => f.Action == ":setvar" || f.Action == ":r");
        Assert.DoesNotContain(result.Findings, f => f.Action == "UPDATE" && f.WillRun);
        Assert.True(result.Findings.Single(f => f.Action == "SELECT").WillRun);
    }

    [Fact]
    public void ModeOff_VariableAloneStillGetsHint()
    {
        var result = WithMode("DELETE FROM $(Table) WHERE Id = 1;", false);
        Assert.NotEmpty(result.ParseErrors);
        Assert.Contains(result.Findings, f => f.Action == "SQLCMD MODE OFF");
    }

    [Fact]
    public void ModeOff_DollarParenInAString_IsJustAString()
    {
        var result = WithMode("SELECT 'price: $(amount)' AS Label;", false);
        Assert.Empty(result.ParseErrors);
        Assert.DoesNotContain(result.Findings, f => f.Action == "SQLCMD MODE OFF");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Fact]
    public void PlainScripts_AreNotTouched()
    {
        Assert.DoesNotContain(SqlSafetyAnalyzer.Analyze("SELECT ':setvar x 1' AS Text").Findings, f => f.Action.StartsWith(":", StringComparison.Ordinal));
    }
}
