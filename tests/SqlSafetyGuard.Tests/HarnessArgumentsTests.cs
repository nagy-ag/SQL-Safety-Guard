using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class HarnessArgumentsTests
{
    [Theory]
    [InlineData("--analyzer", "analyzer.dll")]
    [InlineData("--analyzer", "analyzer.dll", "--legacy-discovery")]
    [InlineData("--analyzer", "analyzer.dll", "--legacy-discovery", "--server", "")]
    [InlineData("--analyzer", "analyzer.dll", "--legacy-discovery", "--server", "disposable", "--campaign", "cases")]
    [InlineData("--analyzer", "analyzer.dll", "--campaign", "cases", "--tests", "tests")]
    [InlineData("--analyzer")]
    public void MissingOrConflictingModesCannotReachSetup(params string[] args) =>
        Assert.Throws<ArgumentException>(() => Args.Parse(args));

    [Fact]
    public void LegacyRequiresExplicitModeAndServer() =>
        Assert.True(Args.Parse(["--analyzer", "analyzer.dll", "--legacy-discovery", "--server", "disposable"]).LegacyDiscovery);
}
