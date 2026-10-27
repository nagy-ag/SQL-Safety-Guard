using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>Scripts that use several temp tables together, and where temp and permanent tables mix.</summary>
public class TempTableTests
{
    private const string TempWorkflow = @"
IF OBJECT_ID('tempdb..#orders') IS NOT NULL DROP TABLE #orders;
CREATE TABLE #orders (Id int PRIMARY KEY, CustomerId int, Qty int);
INSERT #orders SELECT Id, CustomerId, Qty FROM dbo.Orders WHERE Status = 'open';

SELECT c.Id, c.Name INTO #cust
FROM dbo.Customers c
WHERE EXISTS (SELECT 1 FROM #orders o WHERE o.CustomerId = c.Id);

CREATE INDEX ix_cust ON #cust (Id);
ALTER TABLE #orders ADD Flag bit NULL;

UPDATE o SET Flag = 1 FROM #orders o JOIN #cust c ON c.Id = o.CustomerId;
UPDATE #orders SET Qty = c.Id FROM #orders JOIN #cust c ON c.Id = #orders.CustomerId;
DELETE x FROM #cust x JOIN dbo.Orders o ON o.CustomerId = x.Id WHERE o.Qty = 0;

WITH zero AS (SELECT * FROM #orders WHERE Qty = 0) DELETE FROM zero;

MERGE #cust AS t
USING (SELECT DISTINCT CustomerId FROM #orders) AS s ON t.Id = s.CustomerId
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SELECT * INTO ##shared FROM #orders;
DECLARE @summary TABLE (CustomerId int, Total int);
INSERT @summary SELECT CustomerId, SUM(Qty) FROM #orders GROUP BY CustomerId;
UPDATE s SET Total = 0 FROM @summary s JOIN #cust c ON c.Id = s.CustomerId;

TRUNCATE TABLE #cust;
DROP TABLE #orders, #cust, ##shared;
GO
CREATE TABLE #batch2 (x int);
INSERT #batch2 VALUES (1);
UPDATE #batch2 SET x = 2;
EXEC ('INSERT #batch2 VALUES (3)');
DROP TABLE #batch2;
";

    [Fact]
    public void ManyTempTablesAcrossBatches_AreTempOnly()
    {
        var result = SqlSafetyAnalyzer.Analyze(TempWorkflow);

        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.DoesNotContain(result.Findings, f => f.Verdict > Verdict.TempOnly);
        // Every write was seen (not skipped): 18 in the first batch (the 3-table DROP counts 3), 5 in the second.
        Assert.Equal(23, result.Count(Verdict.TempOnly));
    }

    [Fact]
    public void OnePermanentWriteAmongTempWork_IsRedAndOnlyThatStatementIsRed()
    {
        var sql = TempWorkflow.Replace("TRUNCATE TABLE #cust;",
            "UPDATE o SET Qty = t.Qty FROM dbo.Orders o JOIN #orders t ON t.Id = o.Id;\r\nTRUNCATE TABLE #cust;");

        var result = SqlSafetyAnalyzer.Analyze(sql);

        Assert.Equal(Verdict.RealWrite, result.Overall);
        var red = Assert.Single(result.Findings, f => f.Verdict == Verdict.RealWrite);
        Assert.Equal("dbo.Orders", red.ResolvedTarget);
        Assert.Contains("via alias o", red.Note);
    }

    [Theory]
    [InlineData("UPDATE t SET x = 1 FROM #a t JOIN dbo.Orders o ON o.Id = t.Id", Verdict.TempOnly)]
    [InlineData("UPDATE o SET Qty = 1 FROM dbo.Orders o JOIN #a t ON t.Id = o.Id", Verdict.RealWrite)]
    [InlineData("DELETE FROM #a WHERE Id IN (SELECT Id FROM dbo.Orders)", Verdict.TempOnly)]
    [InlineData("INSERT dbo.Orders (Id) SELECT Id FROM #a", Verdict.RealWrite)]
    [InlineData("INSERT #a SELECT * FROM dbo.Orders o JOIN #b b ON b.Id = o.Id", Verdict.TempOnly)]
    [InlineData("MERGE #a t USING #b s ON t.Id = s.Id WHEN MATCHED THEN UPDATE SET t.x = s.x;", Verdict.TempOnly)]
    [InlineData("MERGE dbo.Orders t USING #b s ON t.Id = s.Id WHEN MATCHED THEN DELETE;", Verdict.RealWrite)]
    [InlineData("UPDATE #a SET x = 1 OUTPUT inserted.Id INTO #log", Verdict.TempOnly)]
    [InlineData("UPDATE #a SET x = 1 OUTPUT inserted.Id INTO dbo.AuditLog (OrderId)", Verdict.RealWrite)]
    [InlineData("WITH c AS (SELECT a.* FROM #a a JOIN #b b ON b.Id = a.Id) UPDATE c SET x = 1", Verdict.TempOnly)]
    [InlineData("WITH c AS (SELECT a.* FROM #a a JOIN dbo.Orders o ON o.Id = a.Id) UPDATE c SET x = 1", Verdict.RealWrite)]
    [InlineData("INSERT #a EXEC dbo.usp_GetOrders", Verdict.Unknown)]
    [InlineData("SELECT * INTO tempdb.dbo.scratch FROM #a", Verdict.TempOnly)]
    public void MixedTempAndPermanent(string sql, Verdict expected)
    {
        Assert.Equal(expected, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }
}
