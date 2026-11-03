using System.Diagnostics;
using System.Text;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

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
