using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class AdditionalAnalyzerTests
{
    private static AnalysisResult Analyze(string sql, params string[] allowList) =>
        SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(allowList));

    [Fact]
    public void UsesNewestParser()
    {
        Assert.Matches(@"^TSql\d+Parser$", SqlSafetyAnalyzer.ParserName);
        Assert.NotEqual("TSql160Parser", SqlSafetyAnalyzer.ParserName);
    }

    [Fact]
    public void EmptyText_IsReadOnlyWithNoFindings()
    {
        var result = Analyze("  \r\n ");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        Assert.Empty(result.Findings);
    }

    [Fact]
    public void DeleteViaAliasWithoutFromKeyword_ResolvesToBaseTable()
    {
        var result = Analyze("DELETE x FROM dbo.T x WHERE x.Id = 1");
        var finding = Assert.Single(result.Findings);
        Assert.Equal("dbo.T", finding.ResolvedTarget);
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
    }

    [Fact]
    public void UpdateViaAliasOfTemp_IsTempOnly()
    {
        var result = Analyze("UPDATE f SET X = 1 FROM #fix f JOIN dbo.Orders o ON o.Id = f.Id");
        Assert.Equal(Verdict.TempOnly, result.Overall);
    }

    [Fact]
    public void CteReferencingCte_ResolvesRecursively()
    {
        var result = Analyze("WITH a AS (SELECT * FROM dbo.Orders), b AS (SELECT * FROM a) UPDATE b SET Qty = 0");
        var finding = Assert.Single(result.Findings);
        Assert.Equal("dbo.Orders", finding.ResolvedTarget);
        Assert.Contains("via CTE b", finding.Note);
        Assert.Contains("via CTE a", finding.Note);
    }

    [Fact]
    public void CteOverTempTable_IsTempOnly()
    {
        var result = Analyze("WITH c AS (SELECT * FROM #o) DELETE FROM c");
        Assert.Equal(Verdict.TempOnly, result.Overall);
    }

    [Fact]
    public void RecursiveCte_DoesNotLoopForever()
    {
        var sql = "WITH r AS (SELECT Id FROM dbo.Tree UNION ALL SELECT t.Id FROM dbo.Tree t JOIN r ON r.Id = t.ParentId) DELETE FROM r";
        var result = Analyze(sql);
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Fact]
    public void UpdateViaDerivedTable_ResolvesToBaseTable()
    {
        var result = Analyze("UPDATE d SET Qty = 0 FROM (SELECT * FROM dbo.Orders) d");
        var finding = Assert.Single(result.Findings);
        Assert.Equal("dbo.Orders", finding.ResolvedTarget);
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
    }

    [Fact]
    public void UpdateAliasPointingToCte_ResolvesThroughBoth()
    {
        var result = Analyze("WITH c AS (SELECT * FROM dbo.Orders) UPDATE x SET Qty = 0 FROM c x");
        var finding = Assert.Single(result.Findings);
        Assert.Equal("dbo.Orders", finding.ResolvedTarget);
        Assert.Contains("via alias x", finding.Note);
        Assert.Contains("via CTE c", finding.Note);
    }

    [Fact]
    public void SpExecuteSqlWithLiteral_IsAnalyzed()
    {
        var result = Analyze("EXEC sp_executesql N'UPDATE dbo.Orders SET Qty = 0 WHERE Id = @id', N'@id int', @id = 1");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains("inside dynamic SQL", Assert.Single(result.Findings).Note);
    }

    [Fact]
    public void DynamicSqlWithEscapedQuotes_IsUnescaped()
    {
        var result = Analyze("EXEC('SELECT ''DELETE FROM dbo.Orders''')");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Fact]
    public void DynamicSqlConcatenatedLiterals_AreAnalyzed()
    {
        var result = Analyze("EXEC('DELETE FROM ' + 'dbo.Orders')");
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Fact]
    public void DynamicSqlWithVariablePart_IsUnknown()
    {
        var result = Analyze("DECLARE @t sysname = N'x'; EXEC('DELETE FROM ' + @t)");
        Assert.Equal(Verdict.Unknown, result.Overall);
    }

    [Fact]
    public void NestedDynamicSql_IsLimitedByDepth()
    {
        var sql = "EXEC('EXEC(''EXEC(''''EXEC(''''''''DELETE FROM dbo.Orders'''''''')'''')'')')";
        var limited = SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(maxDynamicSqlDepth: 2));
        Assert.Equal(Verdict.Unknown, limited.Overall);

        var deep = SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(maxDynamicSqlDepth: 5));
        Assert.Equal(Verdict.RealWrite, deep.Overall);
    }

    [Fact]
    public void UnparseableDynamicSql_IsUnknown()
    {
        var result = Analyze("EXEC('DELETE FROM')");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Empty(result.ParseErrors);
    }

    [Fact]
    public void ProcedureNameInVariable_IsUnknown()
    {
        var result = Analyze("DECLARE @p sysname = N'dbo.x'; EXEC @p");
        Assert.Equal(Verdict.Unknown, result.Overall);
    }

    [Fact]
    public void SchemaQualifiedReadOnlyProc_IsReadOnly()
    {
        Assert.Equal(Verdict.ReadOnly, Analyze("EXEC sys.sp_who2; EXEC [sp_spaceused] 'dbo.Orders'").Overall);
    }

    [Fact]
    public void InsertExecOfReadOnlyProc_IsTempOnly()
    {
        Assert.Equal(Verdict.TempOnly, Analyze("INSERT INTO #who EXEC sp_who2").Overall);
    }

    [Fact]
    public void WhileAndTryCatch_AreWalked()
    {
        var result = Analyze("WHILE 1 = 0 BEGIN BEGIN TRY UPDATE #a SET x = 1 END TRY BEGIN CATCH DELETE FROM dbo.Log END CATCH END");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, f => f.Target == "#a" && f.Verdict == Verdict.TempOnly);
        Assert.Contains(result.Findings, f => f.Target == "dbo.Log" && f.Verdict == Verdict.RealWrite);
    }

    [Fact]
    public void ElseBranch_IsWalked()
    {
        Assert.Equal(Verdict.RealWrite, Analyze("IF 1 = 0 SELECT 1 ELSE TRUNCATE TABLE dbo.Orders").Overall);
    }

    [Theory]
    [InlineData("CREATE TABLE #x (Id int)", Verdict.TempOnly)]
    [InlineData("CREATE TABLE dbo.X (Id int)", Verdict.RealWrite)]
    [InlineData("ALTER TABLE #x ADD Name nvarchar(10)", Verdict.TempOnly)]
    [InlineData("ALTER TABLE dbo.X ADD Name nvarchar(10)", Verdict.RealWrite)]
    [InlineData("CREATE INDEX ix ON #x (Id)", Verdict.TempOnly)]
    [InlineData("CREATE INDEX ix ON dbo.X (Id)", Verdict.RealWrite)]
    [InlineData("DROP INDEX ix ON #x", Verdict.TempOnly)]
    [InlineData("DROP INDEX ix ON dbo.X", Verdict.RealWrite)]
    [InlineData("DROP TABLE #a, dbo.B", Verdict.RealWrite)]
    [InlineData("TRUNCATE TABLE dbo.Orders", Verdict.RealWrite)]
    [InlineData("SELECT * INTO tempdb.dbo.x FROM dbo.Orders", Verdict.TempOnly)]
    [InlineData("INSERT INTO ##global SELECT 1", Verdict.TempOnly)]
    public void TableDdlAndTruncate(string sql, Verdict expected)
    {
        Assert.Equal(expected, Analyze(sql).Overall);
    }

    [Theory]
    [InlineData("CREATE VIEW dbo.v AS SELECT 1 AS x")]
    [InlineData("ALTER FUNCTION dbo.f() RETURNS int AS BEGIN RETURN 1 END")]
    [InlineData("CREATE OR ALTER TRIGGER dbo.tr ON dbo.T AFTER INSERT AS DELETE FROM dbo.T")]
    [InlineData("DROP PROCEDURE dbo.p")]
    [InlineData("CREATE SCHEMA s")]
    [InlineData("CREATE LOGIN l WITH PASSWORD = 'x'")]
    [InlineData("CREATE USER u WITHOUT LOGIN")]
    [InlineData("ALTER DATABASE CURRENT SET RECOVERY SIMPLE")]
    [InlineData("GRANT SELECT ON dbo.T TO u")]
    [InlineData("REVOKE SELECT ON dbo.T TO u")]
    [InlineData("DENY SELECT ON dbo.T TO u")]
    public void DefinitionAndSecurityChanges_AreRealWrite(string sql)
    {
        var result = Analyze(sql);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Single(result.Findings);
    }

    [Theory]
    [InlineData("DBCC MEMORYMAP")]
    [InlineData("BACKUP DATABASE x TO DISK = 'c:\\x.bak'")]
    [InlineData("UPDATE STATISTICS dbo.Orders")]
    [InlineData("CHECKPOINT")]
    public void OpaqueMaintenanceCommands_AreUnknown(string sql)
    {
        Assert.Equal(Verdict.Unknown, Analyze(sql).Overall);
    }

    [Theory]
    [InlineData("INSERT OPENQUERY(srv, 'SELECT a FROM t') VALUES (1)")]
    [InlineData("UPDATE OPENQUERY(srv, 'SELECT a FROM t') SET a = 1")]
    [InlineData("DELETE OPENROWSET('MSOLEDBSQL', 'Server=x;Trusted_Connection=yes;', 'SELECT a FROM t')")]
    [InlineData("INSERT INTO OPENDATASOURCE('MSOLEDBSQL', 'Data Source=x;Integrated Security=SSPI').db.dbo.t VALUES (1)")]
    [InlineData("INSERT INTO srv.db.dbo.t VALUES (1)")]
    public void RemoteTargets_AreRealWrite(string sql)
    {
        Assert.Equal(Verdict.RealWrite, Analyze(sql).Overall);
    }

    [Fact]
    public void LinkedServerExec_IsUnknown()
    {
        Assert.Equal(Verdict.Unknown, Analyze("EXEC ('SELECT 1') AT srv").Overall);
    }

    [Fact]
    public void ParseErrorInOneBatch_OtherBatchesStillAnalyzed()
    {
        var sql = "DELETE FROM dbo.Orders\nGO\nSELECT * FROM\nGO\nSELECT 1\n";
        var result = Analyze(sql);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Single(result.ParseErrors);
        Assert.Equal(3, result.ParseErrors[0].Line);
        Assert.Contains(result.Findings, f => f.Action == "SELECT" && f.Line == 5);
    }

    [Fact]
    public void GoInsideComment_DoesNotSplitBatch()
    {
        var result = Analyze("/*\nGO\n*/\nSELECT * FROM\n");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Equal(4, Assert.Single(result.ParseErrors).Line);
    }

    [Fact]
    public void AllowList_DoesNotApplyToDdl()
    {
        Assert.Equal(Verdict.RealWrite, Analyze("DROP TABLE dbo.Staging_Import", "dbo.Staging_*").Overall);
    }

    [Fact]
    public void AllowList_AppliesToTruncateAndSelectInto()
    {
        Assert.Equal(Verdict.AllowListed, Analyze("TRUNCATE TABLE dbo.Staging_A; SELECT * INTO Staging_B FROM dbo.Orders", "dbo.Staging_*").Overall);
    }

    [Fact]
    public void Warnings_AndSummary()
    {
        var result = Analyze("DELETE FROM dbo.A; EXEC dbo.p; UPDATE dbo.B SET x = 1; SELECT 1");
        Assert.Equal("2 real writes, 1 unknown", result.Summary);
        Assert.Equal(3, result.Warnings.Count());
        Assert.Equal(Verdict.RealWrite, result.Warnings.First().Verdict);
    }

    [Fact]
    public void Positions_AreRelativeToAnalyzedText()
    {
        var sql = "SELECT 1;\n  UPDATE dbo.T SET x = 1;";
        var finding = Assert.Single(Analyze(sql).Findings, f => f.Action == "UPDATE");
        Assert.Equal(2, finding.Line);
        Assert.Equal(3, finding.Column);
        Assert.Equal(12, finding.StartOffset);
        Assert.Equal("UPDATE dbo.T SET x = 1;", sql.Substring(finding.StartOffset, finding.Length));
    }

    [Fact]
    public void DynamicSqlInsideBatch_ReportedAtExecLine()
    {
        var sql = "SELECT 1\nGO\nPRINT 1\nEXEC('DELETE FROM dbo.Orders')\n";
        var finding = Assert.Single(Analyze(sql).Findings, f => f.Action == "DELETE");
        Assert.Equal(4, finding.Line);
        Assert.Equal(sql.IndexOf("EXEC", StringComparison.Ordinal), finding.StartOffset);
    }
}
