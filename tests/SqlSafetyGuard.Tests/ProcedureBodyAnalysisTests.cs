using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class ProcedureBodyAnalysisTests
{
    [Theory]
    [InlineData("CREATE")]
    [InlineData("ALTER")]
    [InlineData("CREATE OR ALTER")]
    public void DefinitionAndDeferredEffectsHaveSeparateVerdicts(string verb)
    {
        var sql = verb + " PROCEDURE dbo.p @Id int AS\nBEGIN\nUPDATE dbo.Orders SET X=1 WHERE Id=@Id;\nEXEC dbo.other;\nEND";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        var definition = Assert.Single(result.Findings);
        Assert.Equal(Verdict.RealWrite, definition.Verdict);
        var body = Assert.Single(result.ProcedureBodies);
        Assert.Same(definition, body.Definition);
        Assert.True(body.IsComplete);
        var update = Assert.Single(body.Analysis.Findings, f => f.Action == "UPDATE");
        Assert.Equal(Verdict.RealWrite, update.Verdict);
        Assert.Equal(3, update.Line);
        Assert.Equal(sql.IndexOf("UPDATE", StringComparison.Ordinal), update.StartOffset);
        Assert.Contains(body.Analysis.Findings, f => f.Action == "EXEC" && f.Verdict == Verdict.Unknown);
        Assert.Equal("1 real write", result.Summary);
    }

    [Fact]
    public void TemporaryProcedureBodyDoesNotTurnDefinitionRedOrLeakTransactionState()
    {
        var result = SqlSafetyAnalyzer.Analyze("CREATE PROCEDURE #p AS BEGIN TRAN; DELETE dbo.Orders;\nGO\nSELECT 1;");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Action == "OPEN TRANSACTION");
        var body = Assert.Single(result.ProcedureBodies);
        Assert.Equal(Verdict.RealWrite, body.Analysis.Overall);
        Assert.Contains(body.Analysis.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Theory]
    [InlineData("DELETE dbo.Orders", Verdict.RealWrite)]
    [InlineData("BEGIN DELETE dbo.Orders; END", Verdict.RealWrite)]
    [InlineData("IF @Id=1 DELETE dbo.Orders", Verdict.RealWrite)]
    [InlineData("WHILE @Id=1 BEGIN DELETE dbo.Orders; BREAK; END", Verdict.RealWrite)]
    [InlineData("BEGIN TRY DELETE dbo.Orders; END TRY BEGIN CATCH PRINT 'error'; END CATCH", Verdict.RealWrite)]
    [InlineData("EXEC(N'DELETE dbo.Orders')", Verdict.RealWrite)]
    [InlineData("EXEC sp_executesql N'DELETE dbo.Orders'", Verdict.RealWrite)]
    [InlineData("EXEC(@Sql)", Verdict.Unknown)]
    [InlineData("CREATE TABLE #t(Id int); INSERT #t VALUES(1)", Verdict.TempOnly)]
    [InlineData("DECLARE @t TABLE(Id int); INSERT @t VALUES(1)", Verdict.TempOnly)]
    [InlineData("SELECT 1", Verdict.ReadOnly)]
    public void BodyUsesExistingRules(string statements, Verdict expected)
    {
        var result = SqlSafetyAnalyzer.Analyze("CREATE OR ALTER PROCEDURE dbo.p @Id int, @Sql nvarchar(max) AS\n" + statements);
        Assert.Equal(expected, Assert.Single(result.ProcedureBodies).Analysis.Overall);
        Assert.Single(result.Findings);
    }

    [Fact]
    public void BodiesAcrossBatchesRetainPositionsAndIndependentState()
    {
        const string sql = "SELECT 1;\nGO\nCREATE PROCEDURE dbo.a AS BEGIN TRAN;\nGO\nALTER PROCEDURE dbo.b AS SELECT 1;\nGO\nSELECT 2;";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(2, result.ProcedureBodies.Count);
        Assert.Equal(3, result.ProcedureBodies[0].Definition.Line);
        Assert.Equal(5, result.ProcedureBodies[1].Analysis.Findings[0].Line);
        Assert.DoesNotContain(result.ProcedureBodies[1].Analysis.Findings, f => f.Action == "OPEN TRANSACTION");
        Assert.DoesNotContain(result.Findings, f => f.Action == "OPEN TRANSACTION");
    }

    [Fact]
    public void BrokenDefinitionReportsIncompleteBodyInsteadOfSafeEmptyBody()
    {
        var result = SqlSafetyAnalyzer.Analyze("CREATE OR ALTER PROCEDURE dbo.p AS DELETE FROM;\nGO\nSELECT 1;");
        Assert.NotEmpty(result.ParseErrors);
        var body = Assert.Single(result.ProcedureBodies);
        Assert.False(body.IsComplete);
        Assert.Equal(Verdict.Unknown, body.Analysis.Overall);
    }

    [Theory]
    [InlineData("UPDATE [dbo].[Orders] SET X=1", "[dbo].[Orders]")]
    [InlineData("UPDATE Orders SET X=1", "Orders")]
    [InlineData("DELETE OtherDb.dbo.Orders", "OtherDb.dbo.Orders")]
    [InlineData("UPDATE o SET X=1 FROM dbo.Orders o", "dbo.Orders")]
    [InlineData("WITH c AS (SELECT * FROM dbo.Orders) DELETE c", "dbo.Orders")]
    [InlineData("UPDATE [#alias] SET X=1 FROM dbo.Orders [#alias]", "dbo.Orders")]
    [InlineData("SELECT 1 AS X INTO tempdb.dbo.Permanent", "tempdb.dbo.Permanent")]
    public void DeferredWritesRetainPhysicalTargets(string statements, string expectedTarget)
    {
        var body = Assert.Single(SqlSafetyAnalyzer.Analyze("CREATE PROC dbo.p AS " + statements).ProcedureBodies);
        Assert.Contains(body.Analysis.Findings, f => f.Verdict == Verdict.RealWrite &&
            (f.ResolvedTarget ?? f.Target) == expectedTarget);
    }

    [Fact]
    public void HiddenOutputWriteIsIncludedAndUnknownDynamicSqlDoesNotDisappear()
    {
        var body = Assert.Single(SqlSafetyAnalyzer.Analyze("CREATE PROC dbo.p @sql nvarchar(max) AS " +
            "DECLARE @t TABLE(Id int); INSERT @t OUTPUT inserted.Id INTO dbo.Audit VALUES(1); EXEC(@sql);").ProcedureBodies);
        Assert.Contains(body.Analysis.Findings, f => f.Action == "OUTPUT INTO" && f.Target == "dbo.Audit" && f.Verdict == Verdict.RealWrite);
        Assert.Contains(body.Analysis.Findings, f => f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void DynamicSqlParseFailureMarksBodyIncomplete()
    {
        var body = Assert.Single(SqlSafetyAnalyzer.Analyze("CREATE PROC dbo.p AS EXEC(N'DELETE FROM;')").ProcedureBodies);
        Assert.False(body.IsComplete);
        Assert.False(body.Analysis.IsGreen);
    }

    [Fact]
    public void BodyDoesNotAssumeConnectionSchemaOrDatabaseForAllowListProof()
    {
        var options = new AnalyzerOptions(allowListPatterns: new[] { "app.Allowed", "CurrentDb.dbo.Other" },
            currentDatabase: "CurrentDb", defaultSchema: "app");
        var body = Assert.Single(SqlSafetyAnalyzer.Analyze("CREATE PROC dbo.p AS UPDATE Allowed SET X=1; UPDATE dbo.Other SET X=1;", options).ProcedureBodies);
        Assert.All(body.Analysis.Findings, finding => Assert.Equal(Verdict.RealWrite, finding.Verdict));
        var qualified = Assert.Single(SqlSafetyAnalyzer.Analyze("CREATE PROC dbo.p AS UPDATE CurrentDb.dbo.Other SET X=1;", options).ProcedureBodies);
        Assert.Equal(Verdict.AllowListed, Assert.Single(qualified.Analysis.Findings).Verdict);
    }

    [Fact]
    public void BodyWorkLimitCannotProduceCompleteOrGreenResult()
    {
        var nested = string.Concat(Enumerable.Repeat("BEGIN ", 1001)) + "DELETE dbo.T;" + string.Concat(Enumerable.Repeat(" END", 1001));
        var result = SqlSafetyAnalyzer.Analyze("CREATE PROC dbo.p AS " + nested);
        Assert.False(result.IsGreen);
        Assert.All(result.ProcedureBodies, body => Assert.False(body.IsComplete));
        Assert.Contains(result.Findings, f => f.Action == "NOT ANALYZED");
    }
}
