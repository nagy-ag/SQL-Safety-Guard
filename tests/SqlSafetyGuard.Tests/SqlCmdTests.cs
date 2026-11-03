using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

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
