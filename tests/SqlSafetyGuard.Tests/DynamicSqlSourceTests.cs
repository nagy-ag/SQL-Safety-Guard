using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class DynamicSqlSourceTests
{
    [Theory]
    [InlineData("DECLARE @s nvarchar(max)=N'DELETE dbo.Orders WHERE Id=1'; EXEC sp_executesql @s;", "DELETE dbo.Orders WHERE Id=1")]
    [InlineData("EXEC(N'DELETE dbo.Orders WHERE Name=''x'';');", "DELETE dbo.Orders WHERE Name=''x''")]
    [InlineData("SELECT 1;\nGO\nDECLARE @s nvarchar(max)=N'DELETE dbo.Orders'; EXEC(@s);", "DELETE dbo.Orders")]
    [InlineData("EXEC(N'EXEC(N''DELETE dbo.Orders'');');", "DELETE dbo.Orders")]
    public void DecodedOperationKeepsExecutionAnchorAndExactSource(string sql, string expected)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        var finding = Assert.Single(result.Findings, f => f.Action == "DELETE");
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
        Assert.StartsWith("EXEC", sql.Substring(finding.StartOffset, finding.Length));
        Assert.Equal(expected, string.Concat(finding.SourceRanges.Select(r => sql.Substring(r.StartOffset, r.Length))).TrimEnd(';'));
    }

    [Fact]
    public void CopiesAndAppendKeepOnlyContributingFragments()
    {
        const string sql = "DECLARE @a nvarchar(max)=N'DELETE ', @b nvarchar(max); SET @b=@a; SET @b += N'dbo.Orders'; EXEC(@b);";
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings, f => f.Action == "DELETE");
        Assert.Equal(new[] { "DELETE ", "dbo.Orders" }, finding.SourceRanges.Select(r => sql.Substring(r.StartOffset, r.Length)));
    }

    [Fact]
    public void ReassignmentAndMultipleDecodedStatementsUseTheirOwnSources()
    {
        const string sql = "DECLARE @s nvarchar(max)=N'DELETE dbo.Old; SELECT 1'; EXEC(@s); SET @s=N'UPDATE dbo.New SET Id=1'; EXEC(@s);";
        var findings = SqlSafetyAnalyzer.Analyze(sql).Findings.Where(f => f.Note.Contains("inside dynamic SQL")).ToArray();
        Assert.Equal(new[] { "DELETE dbo.Old;", "SELECT 1", "UPDATE dbo.New SET Id=1" },
            findings.Select(f => string.Concat(f.SourceRanges.Select(r => sql.Substring(r.StartOffset, r.Length)))));
    }

    [Theory]
    [InlineData("EXEC(@s);")]
    [InlineData("DECLARE @s nvarchar(max)=N'DELETE dbo.Old';\nGO\nEXEC(@s);")]
    [InlineData("DECLARE @s nvarchar(max)=N'DELETE dbo.Old'; SELECT @s=SqlText FROM dbo.T; EXEC(@s);")]
    public void UnprovenExecutionDoesNotInventSource(string sql)
    {
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings, f => f.Action == "EXEC");
        Assert.Equal(Verdict.Unknown, finding.Verdict);
        Assert.Empty(finding.SourceRanges);
    }
}
