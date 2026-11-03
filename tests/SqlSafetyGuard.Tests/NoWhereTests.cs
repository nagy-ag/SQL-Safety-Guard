using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class NoWhereTests
{
    private static Finding Single(string sql, string action) =>
        Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings, f => f.Action == action);

    [Fact]
    public void DeleteWithoutWhere_AffectsEveryRow()
    {
        var f = Single("DELETE FROM dbo.Orders", "DELETE");
        Assert.True(f.AffectsAllRows);
        Assert.Contains("no WHERE: affects every row", f.Note);
    }

    [Fact]
    public void UpdateWithoutWhere_AffectsEveryRow()
    {
        Assert.True(Single("UPDATE dbo.Orders SET Qty = 0", "UPDATE").AffectsAllRows);
    }

    [Fact]
    public void WithWhere_NoNote()
    {
        var f = Single("UPDATE dbo.Orders SET Qty = 0 WHERE Id = 1", "UPDATE");
        Assert.False(f.AffectsAllRows);
        Assert.DoesNotContain("no WHERE", f.Note);
    }

    [Theory]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o JOIN #ids i ON i.Id = o.Id", "UPDATE", "rows limited only by the JOIN")]
    [InlineData("UPDATE o SET Qty = 0 FROM #ids i INNER JOIN dbo.Orders o ON i.Id = o.Id LEFT JOIN dbo.Log l ON l.Id = o.Id", "UPDATE", "rows limited only by the JOIN")]
    [InlineData("UPDATE dbo.Orders SET Qty = 0 FROM dbo.Orders JOIN #ids i ON i.Id = Orders.Id", "UPDATE", "rows limited only by the JOIN")]
    [InlineData("DELETE o FROM dbo.Orders o CROSS APPLY (SELECT TOP 1 Id FROM #ids i WHERE i.Id = o.Id) x", "DELETE", "rows limited only by the JOIN")]
    [InlineData("DELETE TOP (10) FROM dbo.Orders", "DELETE", "limited only by TOP")]
    public void NoWhere_VisiblyLimitedRows_AreNotWholeTarget(string sql, string action, string note)
    {
        var f = Single(sql, action);
        Assert.Equal(Verdict.RealWrite, f.Verdict);
        Assert.False(f.AffectsAllRows);
        Assert.Equal(string.Empty, f.AllRowsSummary);
        Assert.Contains(note, f.Note);
    }

    [Theory]
    [InlineData("WITH c AS (SELECT * FROM dbo.Orders WHERE Qty = 0) DELETE FROM c", "DELETE", "rows limited only by the CTE or derived table")]
    [InlineData("UPDATE TOP (100) PERCENT dbo.Orders SET Qty = 0", "UPDATE", "limited only by TOP PERCENT")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o LEFT JOIN #ids i ON i.Id = o.Id", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o LEFT JOIN (dbo.A a JOIN dbo.B b ON b.Id = a.Id) ON a.Id = o.Id", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE o SET Qty = 0 FROM dbo.Orders o CROSS JOIN #ids i", "UPDATE", "the FROM has no inner join")]
    [InlineData("UPDATE dbo.Orders SET Qty = 0 FROM #a a JOIN #b b ON b.Id = a.Id", "UPDATE", "the FROM has no inner join")]
    public void NoWhere_UnprovenLimits_RemainWholeTarget(string sql, string action, string note)
    {
        var f = Single(sql, action);
        Assert.True(f.AffectsAllRows);
        Assert.Contains(note, f.Note);
    }

    [Theory]
    [InlineData("DELETE FROM dbo.Orders", "No WHERE clause: can affect every row of dbo.Orders")]
    [InlineData("WITH c AS (SELECT * FROM dbo.Orders WHERE Qty = 0) DELETE FROM c", "No WHERE clause: only the CTE or derived table limits which rows of c → dbo.Orders change")]
    [InlineData("TRUNCATE TABLE dbo.Log", "Removes every row of dbo.Log")]
    [InlineData("DROP TABLE dbo.Log", "Drops dbo.Log and all its rows")]
    [InlineData("DROP PROCEDURE dbo.p", "Drops dbo.p")]
    public void AllRowsSummary_DescribesTheActualChange(string sql, string summary)
    {
        var f = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings, x => x.AffectsAllRows);
        Assert.Equal(summary, f.AllRowsSummary);
    }

    [Fact]
    public void LegacyTypedConfirmationIsIgnoredForAllWrites()
    {
        var production = new GuardSettings(ExecutionMode.YellowRed, "Production", true, new string[0], new string[0]);
        Assert.False(production.NeedsTypedConfirmation(SqlSafetyAnalyzer.Analyze(
            "UPDATE o SET Qty = 0 FROM dbo.Orders o JOIN #ids i ON i.Id = o.Id")));
Assert.False(production.NeedsTypedConfirmation(SqlSafetyAnalyzer.Analyze("UPDATE dbo.Orders SET Qty = 0")));
    }

    [Fact]
    public void Truncate_RemovesEveryRow()
    {
        var f = Single("TRUNCATE TABLE dbo.Log", "TRUNCATE TABLE");
        Assert.True(f.AffectsAllRows);
        Assert.Contains("removes every row", f.Note);
    }

    [Fact]
    public void CursorUpdate_HasAWhere()
    {
        Assert.False(Single("UPDATE dbo.Orders SET Qty = 0 WHERE CURRENT OF c", "UPDATE").AffectsAllRows);
    }

    [Fact]
    public void TempTableWithoutWhere_GetsNoteButStaysGreen()
    {
        var result = SqlSafetyAnalyzer.Analyze("DELETE FROM #work");
        Assert.Equal(Verdict.TempOnly, result.Overall);
        Assert.True(Assert.Single(result.Findings).AffectsAllRows);
    }
}
