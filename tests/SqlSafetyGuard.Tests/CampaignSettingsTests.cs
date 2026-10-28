using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CampaignSettingsTests
{
    private static readonly AnalyzerHost Host = new(typeof(SqlSafetyAnalyzer).Assembly.Location);

    [Fact]
    public void AppliesAllowListAndRetainsAuthorizedTargets()
    {
        var result = Host.Analyze("UPDATE dbo.Target SET Value=1;", new(AllowListPatterns: ["dbo.Target"]));
        Assert.Null(result.Error);
        Assert.Equal("AllowListed", result.Overall);
        Assert.Contains("dbo.Target", result.Targets);
        Assert.False(result.ShouldWarn);
        Assert.False(result.NeedsTypedConfirmation);
    }

    [Fact]
    public void ResolvesEnvironmentBeforeAnalysis()
    {
        var result = Host.Analyze("UPDATE dbo.Target SET Value=1;", new(CurrentDatabase: "SgPlay_Core", Server: "dev01",
            EnvironmentRules: "dev* | *Core | Dev | Red | true | dbo.Target"));
        Assert.Null(result.Error);
        Assert.Equal("AllowListed", result.Overall);
        Assert.Equal("Red", result.Mode);
        Assert.False(result.NeedsTypedConfirmation);
    }

    [Theory]
    [InlineData("Off", false)]
    [InlineData("Red", false)]
    [InlineData("YellowRed", true)]
    public void WarningModeDoesNotChangeUnknownVerdict(string mode, bool shouldWarn)
    {
        var result = Host.Analyze("EXEC dbo.Opaque;", new(Mode: mode));
        Assert.Equal("Unknown", result.Overall);
        Assert.Equal(shouldWarn, result.ShouldWarn);
        Assert.False(result.NeedsTypedConfirmation);
    }

    [Fact]
    public void ConfigurationFailureIsRetainedAsAnError()
    {
        var result = Host.Analyze("SELECT 1;", new(Mode: "invalid"));
        Assert.Equal("CRASH", result.Overall);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void LoaderAcceptsNoOpsAndReportsBadRowsWithoutAborting()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SgRun_loader_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var file = Path.Combine(directory, "cases.jsonl");
            File.WriteAllLines(file, ["{\"id\":\"empty\",\"sql\":\"\"}", "not json", "{\"id\":\"space\",\"sql\":\"   \"}", "{\"id\":\"empty\",\"sql\":\"SELECT 1\"}"]);
            var cases = CampaignCaseLoader.Load([file], Args.Parse(["--analyzer", typeof(SqlSafetyAnalyzer).Assembly.Location, "--campaign", file, "--campaign-validate", "--out", directory]));
            Assert.Equal(2, cases.Length);
            Assert.Equal(2, File.ReadAllLines(Path.Combine(directory, "rejected.jsonl")).Length);
        }
        finally { Directory.Delete(directory, true); }
    }
}
