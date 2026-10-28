using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class MonitorRegressionTests
{
    [Theory]
    [InlineData("EXEC master.dbo.sp_monitor;")]
    [InlineData("EXEC sp_monitor;")]
    [InlineData("EXEC sys.sp_monitor;")]
    [InlineData("BEGIN EXEC master.dbo.sp_monitor; END;")]
    [InlineData("IF 1=1 EXEC master.dbo.sp_monitor;")]
    [InlineData("DECLARE @iteration int=0; WHILE @iteration<1 BEGIN EXEC master.dbo.sp_monitor; SET @iteration+=1; END;")]
    [InlineData("BEGIN TRY EXEC master.dbo.sp_monitor; END TRY BEGIN CATCH THROW; END CATCH;")]
    [InlineData("EXEC(N'EXEC master.dbo.sp_monitor;');")]
    [InlineData("EXEC sys.sp_executesql N'EXEC master.dbo.sp_monitor;';")]
    [InlineData("EXEC(N'EXEC(N''EXEC master.dbo.sp_monitor;'');');")]
    public void MonitorIsWritingAcrossExecutableWrappers(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Note.Contains("spt_monitor"));
    }

    [Fact]
    public void ApplicationProcedureDoesNotInheritSystemClassification()
    {
        Assert.Equal(Verdict.Unknown, SqlSafetyAnalyzer.Analyze("EXEC app.sp_monitor;").Overall);
    }
}
