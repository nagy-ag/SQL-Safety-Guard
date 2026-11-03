using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>The false alarms and under-warnings found by the statement coverage review (docs/history/2026-10-27-statement-coverage.md).</summary>
public class FalseAlarmTests
{
    [Theory]
    [InlineData("READTEXT dbo.Docs.Body @ptr 0 10")]
    [InlineData("OPEN SYMMETRIC KEY k DECRYPTION BY CERTIFICATE c")]
    [InlineData("CLOSE SYMMETRIC KEY k")]
    [InlineData("OPEN MASTER KEY DECRYPTION BY PASSWORD = 'x'")]
    [InlineData("CLOSE MASTER KEY")]
    [InlineData("LINENO 10")]
    [InlineData("GET CONVERSATION GROUP @g FROM dbo.q")]
    [InlineData("WAITFOR (GET CONVERSATION GROUP @g FROM dbo.q), TIMEOUT 100")]
    public void ReadOrSessionOnly_IsGreen(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Theory]
    [InlineData("DROP STATISTICS #T.st", Verdict.TempOnly)]
    [InlineData("DROP STATISTICS dbo.T.st", Verdict.RealWrite)]
    [InlineData("DROP STATISTICS #A.s1, dbo.B.s2", Verdict.RealWrite)]
    public void DropStatistics_FollowsTempRules(string sql, Verdict expected)
    {
        Assert.Equal(expected, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }
}
