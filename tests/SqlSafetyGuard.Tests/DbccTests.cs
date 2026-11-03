using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class DbccTests
{
    [Theory]
    [InlineData("DBCC SHOW_STATISTICS ('dbo.T', ix)")]
    [InlineData("DBCC INPUTBUFFER (55)")]
    [InlineData("DBCC OPENTRAN")]
    [InlineData("DBCC SQLPERF (LOGSPACE)")]
    [InlineData("DBCC USEROPTIONS")]
    [InlineData("DBCC TRACESTATUS")]
    [InlineData("DBCC CHECKDB WITH NO_INFOMSGS")]
    [InlineData("DBCC CHECKTABLE ('dbo.T')")]
    [InlineData("DBCC CHECKIDENT ('dbo.T', NORESEED)")]
    public void ReadOnlyDbcc_IsGreen(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
    }

    [Theory]
    [InlineData("DBCC FREEPROCCACHE", "plan cache")]
    [InlineData("DBCC DROPCLEANBUFFERS", "buffer pool")]
    [InlineData("DBCC SHRINKFILE (1, 10)", "shrinks")]
    [InlineData("DBCC CHECKIDENT ('dbo.T', RESEED, 0)", "reseed")]
    [InlineData("DBCC CHECKIDENT ('dbo.T')", "reseed")]
    [InlineData("DBCC CHECKDB ('x', REPAIR_ALLOW_DATA_LOSS)", "REPAIR")]
    [InlineData("DBCC TRACEON (1222, -1)", "trace flags")]
    public void ChangingDbcc_IsRed(string sql, string reason)
    {
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings);
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
        Assert.Contains(reason, finding.Note);
    }

    [Fact]
    public void OtherDbcc_StaysYellow()
    {
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze("DBCC MEMORYMAP").Overall);
    }
}
