using System.Text.Json;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class IsolatedFixtureValidationTests
{
    [Fact]
    public void IsolatedSmokeAcceptsFilterAndLimit()
    {
        var options = Args.Parse(["--analyzer", "analyzer.dll", "--isolated-coverage", "features.json", "--filter", "clr-", "--limit", "1"]);
        Assert.Equal(1, options.Limit);
        Assert.Equal("clr-", options.Filter);
        Assert.Throws<ArgumentException>(() => Args.Parse(["--analyzer", "analyzer.dll", "--isolated-coverage", "features.json", "--limit", "-1"]));
    }

    [Fact]
    public void InvalidAndDuplicateFixturesDoNotAbortValidFixtures()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SgRun_fixtures_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var fixture = new { id = "valid", statementTypes = new[] { "SelectStatement" }, setupSql = "", sql = "SELECT 1;", expectedVerdict = "ReadOnly", verifySql = "SELECT 1;", beforeScalar = 1, afterScalar = 1, cleanupSql = "" };
            var file = Path.Combine(directory, "fixtures.json");
            File.WriteAllText(file, JsonSerializer.Serialize(new { schemaVersion = 1, fixtures = new object[] { fixture, new { id = "invalid" }, fixture } }));
            var options = Args.Parse(["--analyzer", typeof(SqlSafetyAnalyzer).Assembly.Location, "--isolated-coverage", file, "--validate-coverage", "--out", directory]);
            Assert.Equal(0, IsolatedCoverageRunner.Run(options, new AnalyzerHost(options.Analyzer)));
            using var rejects = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "isolated-rejected.json")));
            Assert.Equal(2, rejects.RootElement.GetArrayLength());
        }
        finally { Directory.Delete(directory, true); }
    }
}
