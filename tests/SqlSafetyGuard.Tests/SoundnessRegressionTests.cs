using System.Diagnostics;
using System.Text;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class SoundnessRegressionTests
{
    [Theory]
    [InlineData("#Items", "t2", Verdict.RealWrite)]
    [InlineData("t1", "#Items", Verdict.RealWrite)]
    [InlineData("t1", "t2", Verdict.RealWrite)]
    [InlineData("#Items", "#Other", Verdict.TempOnly)]
    [InlineData("##Items", "#Other", Verdict.TempOnly)]
    public void SwitchClassifiesBothTables(string source, string destination, Verdict expected)
    {
        foreach (var partitions in new[] { ("", ""), (" PARTITION 1", ""), ("", " PARTITION 2"), (" PARTITION 1", " PARTITION 2") })
        {
            var result = SqlSafetyAnalyzer.Analyze($"ALTER TABLE {source} SWITCH{partitions.Item1} TO {destination}{partitions.Item2};");
            Assert.Empty(result.ParseErrors);
            Assert.Equal(expected, result.Overall);
            Assert.Contains(result.Findings, finding => finding.Target == source);
            Assert.Contains(result.Findings, finding => finding.Target == destination);
        }
    }

    [Theory]
    [InlineData("RECEIVE TOP (1) * FROM dbo.q", Verdict.RealWrite)]
    [InlineData("RECEIVE TOP (1) * FROM dbo.q INTO @Items", Verdict.RealWrite)]
    [InlineData("GET CONVERSATION GROUP @g FROM dbo.q", Verdict.ReadOnly)]
    public void WaitForPreservesInnerStatementVerdict(string statement, Verdict expected)
    {
        var standalone = SqlSafetyAnalyzer.Analyze(statement + ";");
        var wrapped = SqlSafetyAnalyzer.Analyze($"WAITFOR ({statement}), TIMEOUT 1000;");
        Assert.Empty(standalone.ParseErrors);
        Assert.Empty(wrapped.ParseErrors);
        Assert.Equal(expected, standalone.Overall);
        Assert.Equal(standalone.Overall, wrapped.Overall);
        Assert.Equal(standalone.Findings.Select(finding => finding.Action), wrapped.Findings.Select(finding => finding.Action));
    }

    [Fact]
    public void ReceiveIntoTempTableIsInvalidAndNeverGreen()
    {
        var statement = "RECEIVE TOP (1) * FROM dbo.q INTO #Items";
        foreach (var sql in new[] { statement + ";", $"WAITFOR ({statement}), TIMEOUT 10;" })
        {
            var result = SqlSafetyAnalyzer.Analyze(sql);
            Assert.NotEmpty(result.ParseErrors);
            Assert.Equal(Verdict.Unknown, result.Overall);
        }
    }

    [Fact]
    public void UnknownDbccNameIsClassifiedAsDllUnload()
    {
        var result = SqlSafetyAnalyzer.Analyze("DBCC SSG_UNKNOWN;");
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Note == "unloads an extended procedure DLL");
    }

    [Theory]
    [InlineData("INSERT INTO #audit SELECT Id FROM (DELETE FROM dbo.Customers OUTPUT deleted.Id) AS c")]
    [InlineData("CREATE TABLE #t (Id int); UPDATE [#t] SET Id=2 FROM dbo.Orders AS [#t]")]
    [InlineData("CREATE TABLE #t (Id int); DELETE [#t] FROM dbo.Orders AS [#t]")]
    [InlineData("DBCC CHECKIDENT ('dbo.NORESEED_x', RESEED, 0)")]
    [InlineData("DBCC CHECKIDENT ('dbo.Orders', /* NORESEED */ RESEED, 0)")]
    [InlineData("DBCC SQLPERF ('sys.dm_os_wait_stats', CLEAR)")]
    [InlineData("DBCC SQLPERF ('sys.dm_os_wait_stats', 'CLEAR')")]
    [InlineData("EXEC sys.sp_spaceused @updateusage = 'true'")]
    [InlineData("EXEC dbo.sp_spaceused NULL, 'true'")]
    [InlineData("EXEC sp_spaceused @updateusage = @value")]
    [InlineData("DELETE FROM tempdb.dbo.SharedTable")]
    public void RealWritesAreNeverGreen(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Fact]
    public void NestedModificationOutputIsAlsoClassified()
    {
        var sql = "INSERT #audit SELECT Id FROM (DELETE dbo.Customers OUTPUT deleted.Id) c";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Contains(result.Findings, finding => finding.Action == "DELETE" && finding.Verdict == Verdict.RealWrite);
        Assert.Contains(result.Findings, finding => finding.Action == "INSERT" && finding.Verdict == Verdict.TempOnly);
        var output = SqlSafetyAnalyzer.Analyze("DELETE #t OUTPUT deleted.Id INTO dbo.Audit");
        Assert.Contains(output.Findings, finding => finding.Action == "OUTPUT INTO" && finding.Verdict == Verdict.RealWrite);
    }

    [Fact]
    public void DbccUsesParsedArgumentsBeyondDisplayTruncation()
    {
        var result = SqlSafetyAnalyzer.Analyze("DBCC CHECKDB ('App', /*" + new string('x', 20000) + "*/ REPAIR_ALLOW_DATA_LOSS)");
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Theory]
    [InlineData("DBCC CHECKTABLE ('dbo.REPAIR_Archive')")]
    [InlineData("DBCC CHECKTABLE ([REPAIR_Archive])")]
    [InlineData("DBCC CHECKIDENT ('dbo.Orders', NORESEED)")]
    [InlineData("EXEC sys.sp_spaceused @updateusage = 'false'")]
    [InlineData("SET IMPLICIT_TRANSACTIONS ON; SELECT 1")]
    public void OrdinaryReadsRemainReadOnly(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Theory]
    [InlineData("EXEC OtherDb.custom.sp_executesql N'SELECT 1'")]
    [InlineData("EXEC OtherDb.dbo.sp_help")]
    [InlineData("EXEC OtherDb.custom.xp_readerrorlog")]
    [InlineData("EXEC Remote.master.sys.sp_help")]
    [InlineData("EXEC msdb.custom.sp_help")]
    [InlineData("EXEC master..sp_executesql N'SELECT 1'")]
    public void SystemNamesOutsideSystemNamespaceAreUnknown(string sql)
    {
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    [Theory]
    [InlineData(":setvar cmd \"DELETE FROM dbo.Orders\"\n$(cmd)")]
    [InlineData(":setvar t \"#t; DELETE FROM dbo.Orders\"\nDELETE FROM $(t)")]
    [InlineData(":setvar t \"#t\"\nDELETE FROM $(t)x")]
    [InlineData(":setvar Payload \"'; DELETE FROM dbo.Orders; --\"\nSELECT '$(Payload)'")]
    [InlineData("$(notYetDefined)\n:setvar notYetDefined \"#t\"")]
    [InlineData(":exit (DELETE FROM dbo.Orders)")]
    [InlineData(":exit(SELECT NEXT VALUE FOR dbo.Seq)")]
    [InlineData("SELECT '$(Payload-name)'")]
    [InlineData("SELECT '$(123)'")]
    [InlineData("SELECT '$()'")]
    public void UnexpandedSqlCmdExecutionIsNeverGreen(string sql)
    {
        Assert.False(SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(sqlCmdMode: true)).IsGreen);
    }

    [Fact]
    public void SqlCmdDefinitionsDoNotApplyRetroactivelyAndOffsetsStayOriginal()
    {
        var sql = ":setvar t \"dbo.Orders\"\r\nDELETE FROM $(t);\r\nGO\r\n:setvar t \"#t\"\r\nDELETE FROM $(t);";
        var deletes = SqlSafetyAnalyzer.Analyze(sql).Findings.Where(finding => finding.Action == "DELETE").ToList();
        Assert.Equal(2, deletes.Count);
        Assert.Equal("dbo.Orders", deletes[0].ResolvedTarget);
        Assert.Equal("#t", deletes[1].ResolvedTarget);
        Assert.Equal(2, deletes[0].Line);
        Assert.Equal(sql.IndexOf("DELETE", StringComparison.Ordinal), deletes[0].StartOffset);
    }

    [Theory]
    [InlineData("BEGIN TRAN; IF @@TRANCOUNT = 0 ROLLBACK;")]
    [InlineData("BEGIN TRAN; WHILE 1 = 0 COMMIT;")]
    [InlineData("BEGIN TRAN; RETURN; COMMIT;")]
    [InlineData("SET IMPLICIT_TRANSACTIONS ON; BEGIN TRAN; COMMIT;")]
    [InlineData("BEGIN TRAN; IF 'XACT_STATE' = 'XACT_STATE' COMMIT;")]
    public void PossiblyOpenTransactionsAreReported(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Contains(result.Findings, finding => finding.Action == "OPEN TRANSACTION");
        Assert.False(result.IsGreen);
    }

    [Theory]
    [InlineData("WHILE 1=0 BEGIN TRAN; COMMIT;")]
    [InlineData("BEGIN TRAN\nGO 2\nCOMMIT")]
    [InlineData("IF 'XACT_STATE' = 'XACT_STATE' COMMIT;")]
    [InlineData("BEGIN TRY BEGIN TRAN; COMMIT; END TRY BEGIN CATCH RETURN; END CATCH")]
    [InlineData("BEGIN TRY BEGIN TRAN; COMMIT; END TRY BEGIN CATCH THROW; END CATCH")]
    [InlineData("BEGIN TRAN; THROW 50000, 'stop', 1; COMMIT;")]
    [InlineData("BEGIN TRAN; WHILE 1=1 BEGIN BREAK; COMMIT; END")]
    [InlineData("BEGIN TRAN; WHILE 1=1 BEGIN CONTINUE; COMMIT; END")]
    [InlineData("BEGIN TRAN; GOTO finished; COMMIT; finished: SELECT 1;")]
    public void TransactionUncertaintyIsNotGreen(string sql)
    {
        Assert.False(SqlSafetyAnalyzer.Analyze(sql).IsGreen);
    }

    [Theory]
    [InlineData("CREATE PROCEDURE #p AS DELETE dbo.Orders")]
    [InlineData("ALTER PROCEDURE #p AS DELETE dbo.Orders")]
    [InlineData("DROP PROCEDURE #p")]
    public void TemporaryProcedureDefinitionsDoNotExecuteBodies(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.TempOnly, result.Overall);
    }

    [Fact]
    public void PermanentTempdbObjectsRequireExplicitOptIn()
    {
        Assert.Equal(Verdict.TempOnly, SqlSafetyAnalyzer.Analyze("DELETE tempdb.dbo.X", new AnalyzerOptions(tempdbIsTemp: true)).Overall);
    }

    [Fact]
    public void AllowListHonorsQualificationAndExactIdentifierEnd()
    {
        var options = new AnalyzerOptions(new[] { "dbo.Safe" });
        foreach (var name in new[] { "dbo.[Safe\n]", "Safe", "Linked.Db.dbo.Safe", "OtherDb.dbo.Safe" })
            Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("DELETE " + name, options).Overall);
        var known = new AnalyzerOptions(new[] { "dbo.Safe" }, currentDatabase: "App", defaultSchema: "dbo");
        Assert.Equal(Verdict.AllowListed, SqlSafetyAnalyzer.Analyze("DELETE App.dbo.Safe; DELETE Safe", known).Overall);
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("DELETE OtherDb.dbo.Safe", known).Overall);
    }

    [Theory]
    [InlineData("*")]
    [InlineData("*.*")]
    [InlineData("?.*")]
    public void WildcardOnlyAllowListIsIgnoredVisibly(string pattern)
    {
        var result = SqlSafetyAnalyzer.Analyze("SELECT 1", new AnalyzerOptions(new[] { pattern }));
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Action == "SETTINGS WARNING");
    }
}
