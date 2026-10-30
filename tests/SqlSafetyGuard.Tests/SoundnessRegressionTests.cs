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

public class AnalysisResourceTests
{
    [Theory]
    [InlineData(100)]
    [InlineData(330)]
    [InlineData(700)]
    public void SemicolonFreeScriptsKeepDetailedWriteFindings(int steps)
    {
        var sql = new StringBuilder();
        for (int index = 0; index < steps; index++)
            sql.AppendLine($@"IF OBJECT_ID('tempdb..#work{index}') IS NOT NULL DROP TABLE #work{index}
SELECT o.Id, o.Qty
INTO #work{index}
FROM dbo.Orders o
JOIN dbo.Customers c ON c.Id = o.CustomerId
WHERE o.Qty >= 0
UPDATE work SET Qty = source.Qty
FROM #work{index} work
JOIN dbo.Orders source ON source.Id = work.Id
IF EXISTS (SELECT 1 FROM #work{index} WHERE Qty > 100)
    UPDATE #work{index} SET Qty = Qty - 1 WHERE Qty > 100
SELECT * FROM #work{index}
DROP TABLE #work{index}");
        var script = sql.ToString();
        var green = SqlSafetyAnalyzer.Analyze(script);
        Assert.Empty(green.ParseErrors);
        Assert.True(green.IsGreen);
        var result = SqlSafetyAnalyzer.Analyze(script + "\nUPDATE dbo.Orders SET Qty = Qty + 1 WHERE Id = 7\n" + script);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Action == "UPDATE" && finding.Verdict == Verdict.RealWrite);
    }

    [Theory]
    [InlineData("IF 1=0 ")]
    [InlineData("WHILE 1=0 ")]
    public void ExcessiveControlFlowPrefixesAreRejected(string prefix)
    {
        var result = SqlSafetyAnalyzer.Analyze(string.Concat(Enumerable.Repeat(prefix, 20000)) + "SELECT 1");
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Note?.Contains("too complex") == true);
    }

    [Fact]
    public void ExcessiveElseIfChainIsRejected()
    {
        var sql = string.Join(" ELSE ", Enumerable.Repeat("IF 1=0 SELECT 1", 20000));
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    [Fact]
    public void NestingThatDefeatsStatementCountersIsRejectedBeforeParsing()
    {
        var sql = string.Concat(Enumerable.Repeat("IF 1=1 SELECT 1 ELSE WHILE 1=0 ", 40000)) + "SELECT 1";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Note?.Contains("too complex to analyze: control-flow keyword budget exceeded") == true);
    }

    [Fact]
    public void ParsedAlternatingNestingIsRejectedByWalkerDepthLimit()
    {
        var sql = string.Concat(Enumerable.Repeat("IF 1=1 SELECT 1 ELSE WHILE 1=0 ", 600)) + "SELECT 1";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Note?.Contains("nested too deeply") == true);
    }

    [Theory]
    [InlineData(2500, Verdict.ReadOnly)]
    [InlineData(2501, Verdict.Unknown)]
    public void ControlFlowKeywordBudgetHasExactBatchBoundary(int steps, Verdict expected)
    {
        var sql = string.Concat(Enumerable.Repeat("IF 1=0 SELECT 1 ELSE SELECT 2; ", steps));
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(expected, result.Overall);
        if (expected == Verdict.Unknown)
            Assert.Contains(result.Findings, finding => finding.Note.Contains("control-flow keyword budget exceeded"));
        else
            Assert.Empty(result.ParseErrors);
    }

    [Fact]
    public void ControlFlowKeywordBudgetResetsOnlyAtGo()
    {
        var batch = string.Concat(Enumerable.Repeat("IF 1=0 SELECT 1 ELSE SELECT 2;\n", 1400));
        var result = SqlSafetyAnalyzer.Analyze(batch + "GO\n" + batch);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        Assert.Equal(5600, result.Findings.Count);
    }

    [Fact]
    public void LongFlatListsKeepDetailedFindings()
    {
        var ids = string.Join(",", Enumerable.Range(1, 10000));
        var inList = SqlSafetyAnalyzer.Analyze($"SELECT * FROM dbo.Orders WHERE Id IN ({ids});\nUPDATE dbo.Orders SET Qty = 1 WHERE Id = 7;");
        Assert.Equal(Verdict.RealWrite, inList.Overall);
        Assert.Contains(inList.Findings, finding => finding.Action == "UPDATE" && finding.Verdict == Verdict.RealWrite);

        var values = string.Join(",\n", Enumerable.Range(1, 4000).Select(i => $"({i}, 'x{i}')"));
        Assert.Equal(Verdict.TempOnly,
            SqlSafetyAnalyzer.Analyze($"CREATE TABLE #t (Id int, N varchar(10)); INSERT INTO #t VALUES {values};").Overall);

        var columns = string.Join(", ", Enumerable.Range(1, 3000).Select(i => $"ISNULL(a.c{i}, b.c{i}) AS c{i}"));
        Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze($"SELECT {columns} FROM dbo.A a JOIN dbo.B b ON b.Id = a.Id;").Overall);

        var chain = string.Join(" OR ", Enumerable.Range(0, 20000).Select(i => $"a={i}"));
        Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze("SELECT * FROM dbo.T WHERE " + chain).Overall);
    }

    [Theory]
    [InlineData("IF @flag = 1 RETURN; SELECT 1", true)]
    [InlineData("IF @flag = 1 RETURN; BEGIN TRAN; COMMIT; SELECT 1", true)]
    [InlineData("IF @flag = 1 RETURN; BEGIN TRAN; SELECT 1", false)]
    [InlineData("BEGIN TRAN; IF @flag = 1 RETURN; COMMIT", false)]
    [InlineData("IF @flag = 1 BEGIN BEGIN TRAN; RETURN; END; SELECT 1", false)]
    [InlineData("IF @flag = 1 RETURN; ELSE BEGIN TRAN; SELECT 1", false)]
    public void EarlyReturnPreservesContinuingTransactionPaths(string sql, bool green)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(green, result.IsGreen);
        if (!green)
            Assert.Contains(result.Findings, finding => finding.Action == "OPEN TRANSACTION" || finding.Action == "TRANSACTION FLOW");
    }

    [Fact]
    public void ElseIfChainDoesNotOverflow()
    {
        var sql = string.Join(" ELSE ", Enumerable.Repeat("IF 1=0 SELECT 1", 430)) + " ELSE DELETE dbo.Orders";
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(20000)]
    public void ExcessiveNestingReturnsUnknownWithoutParsing(int depth)
    {
        foreach (var sql in new[] { "SELECT " + new string('(', depth) + "1" + new string(')', depth),
            string.Concat(Enumerable.Repeat("BEGIN ", depth)) + "SELECT 1 " + string.Concat(Enumerable.Repeat("END ", depth)) })
        {
            var result = SqlSafetyAnalyzer.Analyze(sql);
            Assert.Equal(Verdict.Unknown, result.Overall);
            Assert.Contains(result.Findings, finding => finding.Note?.Contains("too complex") == true);
        }
    }

    [Fact]
    public void DoublingCteGraphIsMemoizedAndDeduplicated()
    {
        var sql = new StringBuilder("WITH c0 AS (SELECT Id FROM dbo.Orders)");
        for (int index = 1; index <= 45; index++)
            sql.Append($",c{index} AS (SELECT a.Id FROM c{index - 1} a JOIN c{index - 1} b ON a.Id=b.Id)");
        sql.Append(" DELETE FROM c45");
        var elapsed = Stopwatch.StartNew();
        var result = SqlSafetyAnalyzer.Analyze(sql.ToString());
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Single(result.Findings, finding => finding.Action == "DELETE");
        Assert.True(elapsed.Elapsed < TimeSpan.FromSeconds(2), elapsed.Elapsed.ToString());
    }

    [Fact]
    public async Task CancellationStopsActiveAnalysis()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.Throws<OperationCanceledException>(() => SqlSafetyAnalyzer.Analyze("SELECT 1", cancellationToken: cancellation.Token));
        using var active = new CancellationTokenSource();
        var sql = string.Concat(Enumerable.Repeat("SELECT 1;\n", 200000));
        var work = Task.Run(() => SqlSafetyAnalyzer.Analyze(sql, cancellationToken: active.Token));
        active.CancelAfter(10);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await work.WaitAsync(TimeSpan.FromSeconds(3)));
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(5000)]
    public void RealisticScriptsRemainFastAndFindTheRealWrite(int lines)
    {
        var sql = new StringBuilder();
        for (int index = 0; index < 20; index++) sql.AppendLine($"CREATE TABLE #t{index} (Id int);");
        for (int index = 20; index < lines - 1; index++)
        {
            switch (index % 4)
            {
                case 0: sql.AppendLine("WITH c AS (SELECT Id FROM #t0), d AS (SELECT Id FROM c) UPDATE t SET Id=d.Id FROM #t1 t JOIN d ON t.Id=d.Id;"); break;
                case 1: sql.AppendLine("SELECT d.Id FROM (SELECT Id FROM (SELECT Id FROM #t2) x) d JOIN #t3 t ON d.Id=t.Id WHERE d.Id>0;"); break;
                case 2: sql.AppendLine("MERGE #t4 t USING #t5 s ON t.Id=s.Id WHEN MATCHED THEN UPDATE SET Id=s.Id;"); break;
                default: sql.AppendLine("INSERT #t6 SELECT Id FROM #t7;"); break;
            }
        }
        sql.AppendLine("UPDATE o SET Id=t.Id FROM dbo.Orders o JOIN #t0 t ON o.Id=t.Id WHERE o.Id=1;");
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("UPDATE dbo.Orders SET Id=1 WHERE Id=2;").Overall);
        var elapsed = Stopwatch.StartNew();
        var result = SqlSafetyAnalyzer.Analyze(sql.ToString());
        elapsed.Stop();
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Verdict == Verdict.RealWrite && finding.ResolvedTarget == "dbo.Orders");
        Assert.True(elapsed.Elapsed < TimeSpan.FromSeconds(15), elapsed.Elapsed.ToString());
    }

    [Fact]
    public void LongSingleLineSelectAndLargeBrokenBatchAreBounded()
    {
        var longLine = "SELECT o.Id FROM dbo.Orders o JOIN dbo.Items i ON i.Id=o.Id WHERE " +
            string.Join(" AND ", Enumerable.Repeat("o.Id > 0", 1600));
        Assert.True(longLine.Length > 20000);
        Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze(longLine).Overall);
        var broken = "SELCT broken;\n" + string.Concat(Enumerable.Repeat("DELETE FROM ;\n", 3000));
        var elapsed = Stopwatch.StartNew();
        Assert.False(SqlSafetyAnalyzer.Analyze(broken).IsGreen);
        Assert.True(elapsed.Elapsed < TimeSpan.FromSeconds(3), elapsed.Elapsed.ToString());
    }
}
