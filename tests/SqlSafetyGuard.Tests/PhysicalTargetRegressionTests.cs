using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class PhysicalTargetRegressionTests
{
    [Fact]
    public void SchemaTransferResolvesTheMovedObject()
    {
        var result = SqlSafetyAnalyzer.Analyze("ALTER SCHEMA app TRANSFER dbo.IndSink;");
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, finding => finding.ResolvedTarget == "dbo.IndSink");
    }

    [Theory]
    [InlineData("DROP STATISTICS dbo.IndTarget.IndValueStats;")]
    [InlineData("DROP INDEX dbo.IndTarget.IndValueIndex;")]
    public void PhysicalObjectChangesResolveToTheirTable(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Contains(result.Findings, finding => finding.ResolvedTarget == "dbo.IndTarget");
    }
}
