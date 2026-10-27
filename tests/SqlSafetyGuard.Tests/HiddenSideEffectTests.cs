using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>Remote pass-through sources (yellow) and NEXT VALUE FOR (red) inside statements that look read-only.</summary>
public class HiddenSideEffectTests
{
    private const string OpenQueryWrites = "SELECT * FROM OPENQUERY(srv, 'EXEC dbo.usp_Cleanup')";

    [Fact]
    public void OpenQuerySource_IsUnknown()
    {
        var result = SqlSafetyAnalyzer.Analyze(OpenQueryWrites);
        Assert.Equal(Verdict.Unknown, result.Overall);
        var finding = Assert.Single(result.Findings, f => f.Verdict == Verdict.Unknown);
        Assert.Equal("OPENQUERY", finding.Action);
        Assert.Contains("linked server srv", finding.Note);
        Assert.Equal(OpenQueryWrites.IndexOf("OPENQUERY", StringComparison.Ordinal), finding.StartOffset);
    }

    [Theory]
    [InlineData("SELECT * FROM OPENROWSET('MSOLEDBSQL', 'Server=x;Trusted_Connection=yes;', 'EXEC dbo.p')", "OPENROWSET")]
    [InlineData("SELECT * FROM OPENDATASOURCE('MSOLEDBSQL', 'Data Source=x;Integrated Security=SSPI').db.dbo.t", "OPENDATASOURCE")]
    [InlineData("WITH c AS (SELECT * FROM OPENQUERY(srv, 'SELECT 1 AS a')) SELECT * FROM c", "OPENQUERY")]
    [InlineData("SELECT o.Id FROM dbo.Orders o WHERE o.Id IN (SELECT a FROM OPENQUERY(srv, 'SELECT 1 AS a'))", "OPENQUERY")]
    [InlineData("IF EXISTS (SELECT 1 FROM OPENQUERY(srv, 'SELECT 1 AS a')) PRINT 'yes'", "OPENQUERY")]
    [InlineData("WHILE EXISTS (SELECT 1 FROM OPENQUERY(srv, 'SELECT 1 AS a')) BREAK", "OPENQUERY")]
    [InlineData("DECLARE @n int = (SELECT COUNT(*) FROM OPENQUERY(srv, 'SELECT 1 AS a'))", "OPENQUERY")]
    [InlineData("DECLARE c CURSOR FOR SELECT * FROM OPENQUERY(srv, 'SELECT 1 AS a')", "OPENQUERY")]
    public void RemoteSources_AreUnknown(string sql, string action)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == action && f.Verdict == Verdict.Unknown);
    }

    [Fact]
    public void TempTargetFedByRemoteSource_IsUnknown()
    {
        var result = SqlSafetyAnalyzer.Analyze("INSERT #t SELECT * FROM OPENQUERY(srv, 'SELECT 1 AS a')");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "INSERT" && f.Verdict == Verdict.TempOnly);
    }

    [Theory]
    [InlineData("UPDATE OPENQUERY(srv, 'SELECT a FROM t') SET a = 1")]
    [InlineData("INSERT OPENQUERY(srv, 'SELECT a FROM t') VALUES (1)")]
    [InlineData("DELETE OPENQUERY(srv, 'SELECT a FROM t')")]
    public void RemoteWriteTarget_IsReportedOnceAsRealWrite(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Single(result.Findings);
    }

    [Fact]
    public void OpenRowsetBulk_ReadsAFile_IsReadOnly()
    {
        var result = SqlSafetyAnalyzer.Analyze(@"SELECT BulkColumn FROM OPENROWSET(BULK 'C:\data\x.json', SINGLE_CLOB) AS j");
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Fact]
    public void FourPartNameRead_IsReadOnly()
    {
        Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze("SELECT * FROM srv.db.dbo.Orders").Overall);
    }

    [Fact]
    public void OpenQueryInsideLiteralDynamicSql_IsUnknown()
    {
        var result = SqlSafetyAnalyzer.Analyze("EXEC ('SELECT * FROM OPENQUERY(srv, ''EXEC dbo.p'')')");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "OPENQUERY" && f.Note.Contains("inside dynamic SQL"));
    }

    [Fact]
    public void NextValueFor_IsRealWrite()
    {
        var result = SqlSafetyAnalyzer.Analyze("SELECT NEXT VALUE FOR dbo.OrderNumbers AS NewNumber");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var finding = Assert.Single(result.Findings, f => f.Verdict == Verdict.RealWrite);
        Assert.Equal("NEXT VALUE FOR", finding.Action);
        Assert.Equal("dbo.OrderNumbers", finding.Target);
        Assert.Contains("ROLLBACK does not undo it", finding.Note);
    }

    [Theory]
    [InlineData("DECLARE @id int = NEXT VALUE FOR dbo.seq")]
    [InlineData("DECLARE @id int; SET @id = NEXT VALUE FOR dbo.seq")]
    [InlineData("INSERT #t (Id) VALUES (NEXT VALUE FOR dbo.seq)")]
    [InlineData("UPDATE #t SET Id = NEXT VALUE FOR dbo.seq")]
    [InlineData("SELECT NEXT VALUE FOR dbo.seq AS Id INTO #t FROM dbo.Orders")]
    [InlineData("IF NEXT VALUE FOR dbo.seq > 100 PRINT 'big'")]
    [InlineData("EXEC ('SELECT NEXT VALUE FOR dbo.seq')")]
    public void NextValueFor_AnywhereThatRuns_IsRealWrite(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "NEXT VALUE FOR");
    }

    [Fact]
    public void NextValueForInTableDefault_DoesNotRunNow_StaysTempOnly()
    {
        var result = SqlSafetyAnalyzer.Analyze("CREATE TABLE #t (Id int DEFAULT (NEXT VALUE FOR dbo.seq), Name nvarchar(10))");
        Assert.Equal(Verdict.TempOnly, result.Overall);
    }

    [Theory]
    [InlineData("CREATE PROCEDURE dbo.p AS SELECT NEXT VALUE FOR dbo.seq")]
    [InlineData("CREATE VIEW dbo.v AS SELECT * FROM OPENQUERY(srv, 'SELECT 1 AS a')")]
    public void ModuleBodies_AreStillNotWalked(string sql)
    {
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings);
        Assert.StartsWith("CREATE", finding.Action);
    }
}
