using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>The 23 required cases from the specification.</summary>
public class RequiredCasesTests
{
    private static AnalysisResult Analyze(string sql, params string[] allowList) =>
        SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(allowList));

    [Fact]
    public void Case01_Select_IsReadOnly()
    {
        var result = Analyze("SELECT * FROM dbo.Orders");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        Assert.Empty(result.ParseErrors);
    }

    [Fact]
    public void Case02_SelectIntoTempThenDeleteTemp_IsTempOnly()
    {
        var result = Analyze("SELECT * INTO #o FROM dbo.Orders; DELETE FROM #o WHERE Qty = 0");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "SELECT INTO" && f.Target == "#o" && f.Verdict == Verdict.TempOnly);
        Assert.Contains(result.Findings, f => f.Action == "DELETE" && f.Target == "#o" && f.Verdict == Verdict.TempOnly);
    }

    [Fact]
    public void Case03_TruncateAndDropTemp_IsTempOnly()
    {
        var result = Analyze("TRUNCATE TABLE #stage; DROP TABLE IF EXISTS #stage");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.Equal(2, result.Findings.Count);
    }

    [Fact]
    public void Case04_InsertTableVariable_IsTempOnly()
    {
        var result = Analyze("DECLARE @t TABLE (Id int); INSERT @t SELECT Id FROM dbo.Orders");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "INSERT" && f.Target == "@t" && f.Verdict == Verdict.TempOnly);
    }

    [Fact]
    public void Case05_UpdateViaAlias_ResolvesToBaseTable()
    {
        var result = Analyze("UPDATE o SET Qty = 0 FROM dbo.Orders o JOIN #fix f ON f.Id = o.Id");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var finding = Assert.Single(result.Findings);
        Assert.Equal("UPDATE", finding.Action);
        Assert.Equal("o", finding.Target);
        Assert.Equal("dbo.Orders", finding.ResolvedTarget);
        Assert.Contains("via alias o", finding.Note);
    }

    [Fact]
    public void Case06_SelectIntoPermanentTable_IsRealWrite()
    {
        var result = Analyze("SELECT * INTO dbo.Orders_Backup FROM dbo.Orders");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Equal("dbo.Orders_Backup", Assert.Single(result.Findings).Target);
    }

    [Fact]
    public void Case07_OutputIntoPermanentTable_IsRealWrite()
    {
        var result = Analyze("INSERT @t OUTPUT inserted.* INTO dbo.AuditLog SELECT 1");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "OUTPUT INTO" && f.Target == "dbo.AuditLog" && f.Verdict == Verdict.RealWrite);
        Assert.Contains(result.Findings, f => f.Action == "INSERT" && f.Target == "@t" && f.Verdict == Verdict.TempOnly);
    }

    [Fact]
    public void Case08_DeleteThroughCte_ResolvesToBaseTable()
    {
        var result = Analyze("WITH c AS (SELECT * FROM dbo.Orders) DELETE FROM c");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var finding = Assert.Single(result.Findings);
        Assert.Equal("c", finding.Target);
        Assert.Equal("dbo.Orders", finding.ResolvedTarget);
        Assert.Contains("via CTE c", finding.Note);
    }

    [Fact]
    public void Case09_ExecProcedure_IsUnknown()
    {
        var result = Analyze("EXEC dbo.usp_Cleanup");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Equal("dbo.usp_Cleanup", Assert.Single(result.Findings).Target);
    }

    [Fact]
    public void Case10_SpExecuteSqlWithVariable_IsUnknown()
    {
        var result = Analyze("EXEC sp_executesql @sql");
        Assert.Equal(Verdict.Unknown, result.Overall);
    }

    [Fact]
    public void Case11_LiteralDynamicSqlDelete_IsRealWriteInsideDynamicSql()
    {
        var result = Analyze("EXEC('DELETE FROM dbo.Orders')");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var finding = Assert.Single(result.Findings);
        Assert.Equal("DELETE", finding.Action);
        Assert.Equal("dbo.Orders", finding.Target);
        Assert.Contains("inside dynamic SQL", finding.Note);
        Assert.Equal(1, finding.Line);
        Assert.Equal(0, finding.StartOffset);
    }

    [Fact]
    public void Case12_LiteralDynamicSqlOnTemp_IsTempOnly()
    {
        var result = Analyze("EXEC('DELETE FROM #t')");
        Assert.Equal(Verdict.TempOnly, result.Overall);
    }

    [Fact]
    public void Case13_InsertExec_IsUnknown()
    {
        var result = Analyze("INSERT INTO #t EXEC dbo.usp_GetData");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "INSERT" && f.Verdict == Verdict.TempOnly);
        Assert.Contains(result.Findings, f => f.Target == "dbo.usp_GetData" && f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void Case14_DeleteNestedInIfBlock_IsRealWrite()
    {
        var result = Analyze("IF 1 = 1 BEGIN DELETE FROM dbo.Orders END");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Equal("dbo.Orders", Assert.Single(result.Findings).Target);
    }

    [Fact]
    public void Case15_SecondGoBatch_ReportsCorrectLine()
    {
        var sql = "SELECT 1\r\nGO\r\nSELECT 2\r\nDELETE FROM dbo.Orders\r\nGO\r\n";
        var result = Analyze(sql);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var delete = Assert.Single(result.Findings, f => f.Action == "DELETE");
        Assert.Equal(4, delete.Line);
        Assert.Equal(1, delete.Column);
        Assert.Equal(sql.IndexOf("DELETE", StringComparison.Ordinal), delete.StartOffset);
    }

    [Fact]
    public void Case16_AllowListedTable_IsAllowListed()
    {
        var result = Analyze("UPDATE dbo.Staging_Import SET X = 1", "dbo.Staging_*");
        Assert.Equal(Verdict.AllowListed, result.Overall);
        Assert.True(result.IsGreen);
    }

    [Fact]
    public void Case17_CreateProcedure_IsRealWriteWithoutBody()
    {
        var result = Analyze("CREATE PROCEDURE dbo.p AS DELETE FROM dbo.Orders");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var finding = Assert.Single(result.Findings);
        Assert.Equal("CREATE PROCEDURE", finding.Action);
        Assert.Equal("dbo.p", finding.Target);
    }

    [Fact]
    public void Case18_MergeIntoPermanentTable_IsRealWrite()
    {
        var result = Analyze("MERGE dbo.Target t USING #src s ON t.Id = s.Id WHEN MATCHED THEN UPDATE SET t.X = s.X;");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Equal("dbo.Target", Assert.Single(result.Findings).Target);
    }

    [Fact]
    public void Case19_MergeIntoTemp_IsTempOnly()
    {
        var result = Analyze("MERGE #t t USING dbo.Src s ON t.Id = s.Id WHEN MATCHED THEN DELETE WHEN NOT MATCHED THEN INSERT (Id) VALUES (s.Id);");
        Assert.Equal(Verdict.TempOnly, result.Overall);
    }

    [Fact]
    public void Case20_SyntaxError_IsUnknownWithParseError()
    {
        var result = Analyze("SELECT * FROM");
        Assert.Equal(Verdict.Unknown, result.Overall);
        var error = Assert.Single(result.ParseErrors);
        Assert.Equal(1, error.Line);
        Assert.False(string.IsNullOrEmpty(error.Message));
    }

    [Fact]
    public void Case21_CommentsAndStrings_AreReadOnly()
    {
        var result = Analyze("-- DELETE FROM dbo.Orders\r\n/* DELETE FROM dbo.Orders */\r\nSELECT 'DELETE FROM dbo.Orders'");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Fact]
    public void Case22_SessionAndTransactionStatements_AreReadOnly()
    {
        var result = Analyze("SET NOCOUNT ON; BEGIN TRAN; SELECT 1; COMMIT");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        Assert.Equal(4, result.Findings.Count);
    }

    [Fact]
    public void Case23_SpHelp_IsReadOnly()
    {
        var result = Analyze("EXEC sp_help 'dbo.Orders'");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }
}
