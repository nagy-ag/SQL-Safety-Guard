using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class VariableDynamicSqlTests
{
    [Theory]
    [InlineData("DECLARE @sql nvarchar(max) = N'DELETE dbo.Orders'; EXEC(@sql);", Verdict.RealWrite)]
    [InlineData("DECLARE @sql nvarchar(max); SET @sql = N'DELETE dbo.Orders'; EXEC sys.sp_executesql @sql;", Verdict.RealWrite)]
    [InlineData("DECLARE @sql varchar(max) = 'DELETE dbo.Orders'; EXEC(@sql);", Verdict.RealWrite)]
    [InlineData("DECLARE @t nvarchar(128) = N'dbo.Orders', @sql nvarchar(max); SET @sql = N'DELETE ' + @t; EXEC(@sql);", Verdict.RealWrite)]
    [InlineData("DECLARE @a nvarchar(max) = N'DELETE ', @b nvarchar(max); SET @b = @a; SET @b += N'dbo.Orders'; EXEC(@b);", Verdict.RealWrite)]
    [InlineData("DECLARE @t nvarchar(128) = N'dbo.Orders'; EXEC(N'DELETE ' + @t);", Verdict.RealWrite)]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; EXEC sp_executesql @stmt = @sql;", Verdict.ReadOnly)]
    [InlineData("BEGIN DECLARE @sql nvarchar(max) = N'SELECT 1'; EXEC(@sql); END;", Verdict.ReadOnly)]
    [InlineData("EXEC(N'DECLARE @sql nvarchar(max) = N''DELETE dbo.Orders''; EXEC(@sql);');", Verdict.RealWrite)]
    [InlineData("DECLARE @sql nvarchar(max) = N'CREATE TABLE #t(Id int); INSERT #t VALUES(1);'; EXEC(@sql);", Verdict.TempOnly)]
    public void ProvenStringsAreAnalyzed(string sql, Verdict expected)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(expected, result.Overall);
        Assert.Contains(result.Findings, f => f.Note?.Contains("inside dynamic SQL") == true);
    }

    [Theory]
    [InlineData("EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max); EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; SET @sql = NULL; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; SELECT @sql = SqlText FROM dbo.Templates; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; UPDATE dbo.Templates SET @sql = SqlText; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; EXEC dbo.GetSql @sql OUTPUT; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; EXEC @sql = dbo.GetSql; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; IF @flag = 1 SET @sql = N'DELETE dbo.Orders'; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; WHILE 1=1 BEGIN EXEC(@sql); SET @sql=N'DELETE dbo.Orders'; END;")]
    [InlineData("DECLARE @sql nvarchar(max) = N'DELETE dbo.Orders'; GOTO run; SET @sql = N'SELECT 1'; run: EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; BEGIN TRY SET @sql = dbo.GetSql(); END TRY BEGIN CATCH EXEC(@sql); END CATCH;")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1';\nGO\nEXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; EXEC(N'EXEC(@sql)');")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; SET @SQL = N'DELETE dbo.Orders'; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; SET @sql = REPLACE(@sql,N'SELECT',N'DELETE'); EXEC(@sql);")]
    [InlineData("DECLARE @sql char(100) = 'SELECT 1'; EXEC(@sql);")]
    [InlineData("DECLARE @sql varchar(max) = N'SELECT N''漢'''; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'SELECT 1'; DECLARE @SQL nvarchar(max) = N'DELETE dbo.Orders'; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar(max) = N'DELETE dbo.Orders'; SELECT 1; EXEC(@sql);")]
    [InlineData("DECLARE @sql nvarchar = N'SELECT 1'; EXEC(@sql);")]
    public void UnprovenValuesStayUnknown(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Contains(result.Findings, f => f.Verdict == Verdict.Unknown && f.Action == "EXEC");
        Assert.False(result.Overall.IsGreen());
    }

    [Fact]
    public void AssignmentTruncationIsRespected()
    {
        // Truncation removes the comment terminator and must never expose the apparent SELECT as proof.
        var result = SqlSafetyAnalyzer.Analyze("DECLARE @sql nvarchar(10) = N'SELECT 1; /* complete */'; EXEC(@sql);");
        Assert.Equal(Verdict.Unknown, result.Overall);
    }

    [Fact]
    public void IntermediateConcatenationTruncationIsNotHiddenByMaxDestination()
    {
        var prefix = "SELECT 1; /*" + new string('x', 3989);
        var sql = $"DECLARE @a nvarchar(4000) = N'{prefix}', @b nvarchar(4000) = N'*/', @sql nvarchar(max); SET @sql = @a + @b; EXEC(@sql);";
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    [Fact]
    public void ReassignmentAndSeparateBatchesDoNotReuseOldText()
    {
        var result = SqlSafetyAnalyzer.Analyze("DECLARE @sql nvarchar(max)=N'SELECT 1'; SET @sql=N'DELETE dbo.Orders'; EXEC(@sql);\nGO\nDECLARE @sql nvarchar(max)=N'SELECT 2'; EXEC(@sql);");
        Assert.Contains(result.Findings, f => f.Action == "DELETE" && f.DisplayTarget == "dbo.Orders");
        Assert.Contains(result.Findings, f => f.Action == "SELECT" && f.Note.Contains("inside dynamic SQL"));
    }

    [Fact]
    public void FindingsPointAtExecAndKeepRowScope()
    {
        var result = SqlSafetyAnalyzer.Analyze("DECLARE @sql nvarchar(max)=N'DELETE dbo.Orders';\nEXEC(@sql);");
        var finding = Assert.Single(result.Findings, f => f.Action == "DELETE");
        Assert.Equal(2, finding.Line);
        Assert.True(finding.AffectsAllRows);
        Assert.Equal("dbo.Orders", finding.DisplayTarget);
    }

    [Theory]
    [InlineData("DELETE [dbo].[Orders]")]
    [InlineData("DELETE Orders")]
    [InlineData("DELETE OtherDb.dbo.Orders")]
    [InlineData("DELETE tempdb.dbo.Orders")]
    [InlineData("WITH c AS (SELECT * FROM dbo.Orders) DELETE c")]
    [InlineData("UPDATE x SET Qty=0 FROM dbo.Orders x")]
    [InlineData("DELETE t OUTPUT deleted.Id INTO dbo.Log FROM #t t")]
    [InlineData("SELECT * INTO dbo.Copy FROM #t")]
    [InlineData("SELECT NEXT VALUE FOR dbo.Seq")]
    public void EveryResolvedWriteKeepsItsExistingClassification(string body)
    {
        var escaped = body.Replace("'", "''");
        var sql = $"DECLARE @sql nvarchar(max)=N'{escaped}'; EXEC(@sql);";
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze(sql).Overall);
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("BEGIN " + sql + " END").Overall);
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("BEGIN TRAN; " + sql + " COMMIT;").Overall);
        Assert.False(SqlSafetyAnalyzer.Analyze("IF 1=1 BEGIN " + sql + " END").Overall.IsGreen());
        Assert.False(SqlSafetyAnalyzer.Analyze("WHILE 1=0 BEGIN " + sql + " END").Overall.IsGreen());
        Assert.False(SqlSafetyAnalyzer.Analyze("BEGIN TRY " + sql + " END TRY BEGIN CATCH PRINT 1; END CATCH;").Overall.IsGreen());
    }

    [Fact]
    public void DepthAndExpansionLimitsStayClosed()
    {
        var sql = "DECLARE @sql nvarchar(max)=N'SELECT 1'; EXEC(@sql);";
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(maxDynamicSqlDepth: 0)).Overall);
        var large = "DECLARE @sql nvarchar(max)=N'" + new string(' ', 3000) + "SELECT 1';" +
            string.Concat(Enumerable.Repeat("SET @sql = @sql + @sql;", 12)) + "EXEC(@sql);";
        Assert.False(SqlSafetyAnalyzer.Analyze(large).Overall.IsGreen());
    }

    [Fact]
    public void ProcedureBodiesUseTheirOwnVariableProof()
    {
        var result = SqlSafetyAnalyzer.Analyze("CREATE PROCEDURE dbo.P AS BEGIN DECLARE @sql nvarchar(max)=N'DELETE dbo.Orders'; EXEC(@sql); END;");
        Assert.Equal(Verdict.RealWrite, Assert.Single(result.ProcedureBodies).Analysis.Overall);
    }
}
