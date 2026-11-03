using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class SystemProcedureTests
{
    [Theory]
    [InlineData("EXEC sp_rename 'dbo.T', 'T2'", "renames an object")]
    [InlineData("EXEC sp_configure 'show advanced options', 1", "changes server configuration")]
    [InlineData("EXEC xp_cmdshell 'dir'", "operating-system command")]
    [InlineData("EXEC msdb.dbo.sp_add_job @job_name = N'x'", "SQL Agent job")]
    [InlineData("EXEC msdb.dbo.sp_send_dbmail @recipients = 'a@b.c'", "sends an email")]
    [InlineData("EXEC [master].[sys].[sp_addsrvrolemember] 'l', 'sysadmin'", "server role membership")]
    public void KnownWriters_AreRed(string sql, string reason)
    {
        var finding = Assert.Single(SqlSafetyAnalyzer.Analyze(sql).Findings);
        Assert.Equal(Verdict.RealWrite, finding.Verdict);
        Assert.Contains(reason, finding.Note);
    }

    [Theory]
    [InlineData("EXEC sp_set_session_context @key = N'a', @value = 1")]
    [InlineData("EXEC sp_readerrorlog")]
    [InlineData("EXEC msdb.dbo.sp_help_job")]
    public void MoreReadOnlyProcedures_AreGreen(string sql)
    {
        Assert.Equal(Verdict.ReadOnly, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    [Fact]
    public void UnknownProcedure_StaysYellow()
    {
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze("EXEC dbo.usp_Whatever").Overall);
    }

    [Fact]
    public void InsertExecOfKnownWriter_IsRed()
    {
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("INSERT #r EXEC xp_cmdshell 'dir'").Overall);
    }
}
