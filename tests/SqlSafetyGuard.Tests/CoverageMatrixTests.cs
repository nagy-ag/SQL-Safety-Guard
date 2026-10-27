using System.Text.Json;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CoverageMatrixTests
{
    [Fact]
    public void EveryKindHasRuntimeFixtureOrExplicitLimitation()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "Coverage");
        var (manifest, fixtures) = CoverageCorpus.Load(directory);
        var linked = fixtures.SelectMany(fixture => fixture.StatementTypes).ToHashSet();
        foreach (var filename in new[] { "isolated-fixtures.json", "additional-isolated-fixtures.json", "remaining-isolated-fixtures.json" })
        {
            using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, filename)));
            foreach (var fixture in document.RootElement.GetProperty("fixtures").EnumerateArray())
            {
                var parsed = CoverageCorpus.StatementTypes(fixture.GetProperty("sql").GetString()!);
                foreach (var kindElement in fixture.GetProperty("statementTypes").EnumerateArray())
                {
                    var kind = kindElement.GetString()!;
                    Assert.Contains(kind, parsed);
                    Assert.Contains(manifest.Entries, entry => entry.StatementType == kind);
                    linked.Add(kind);
                }
            }
        }
        linked.UnionWith(new[] { "KillStatement", "ShutdownStatement" });
        using var limitations = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "runtime-limitations.json")));
        var reasons = limitations.RootElement.GetProperty("limitations").EnumerateArray().ToArray();
        Assert.Equal(reasons.Length, reasons.Select(item => item.GetProperty("statementType").GetString()).Distinct().Count());
        foreach (var entry in manifest.Entries.Where(entry => !linked.Contains(entry.StatementType)))
            Assert.Contains(reasons, item => item.GetProperty("statementType").GetString() == entry.StatementType &&
                !string.IsNullOrWhiteSpace(item.GetProperty("reason").GetString()));
        Assert.All(reasons, reason => Assert.Contains(manifest.Entries,
            entry => entry.StatementType == reason.GetProperty("statementType").GetString()));
    }
}

