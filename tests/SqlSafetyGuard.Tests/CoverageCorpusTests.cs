using System.Text.Json;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CoverageCorpusTests
{
    [Fact]
    public void CorpusMatchesPinnedInventoryAndParsedKinds()
    {
        var (manifest, fixtures) = CoverageCorpus.Load(Path.Combine(AppContext.BaseDirectory, "Coverage"));
        Assert.Equal(334, manifest.Entries.Length);
        Assert.NotEmpty(fixtures);
        Assert.Contains(manifest.Entries, entry => entry.Status != "fixture");
    }

    [Fact]
    public void DuplicateFixtureCannotInflateCoverage()
    {
        var (manifest, fixtures) = CoverageCorpus.Load(Path.Combine(AppContext.BaseDirectory, "Coverage"));
        Assert.Throws<InvalidDataException>(() => CoverageCorpus.Validate(manifest, fixtures.Append(fixtures[0]).ToArray()));
    }

    [Fact]
    public void SetupOnlyTypeCannotEarnCoverage()
    {
        var (manifest, fixtures) = CoverageCorpus.Load(Path.Combine(AppContext.BaseDirectory, "Coverage"));
        var modified = fixtures.Select(fixture => fixture.Id == "read-select" ? fixture with { Sql = "PRINT N'hello';" } : fixture).ToArray();
        Assert.Throws<InvalidDataException>(() => CoverageCorpus.Validate(manifest, modified));
    }

    [Fact]
    public void BrokenSqlCannotBecomeReadOnlyEvidence() =>
        Assert.Throws<InvalidDataException>(() => CoverageCorpus.StatementTypes("SELECT FROM;"));

    [Fact]
    public void ExclusionsNeedExplicitReasons()
    {
        var (manifest, fixtures) = CoverageCorpus.Load(Path.Combine(AppContext.BaseDirectory, "Coverage"));
        var entry = manifest.Entries.First(item => item.Status == "missing");
        var modified = manifest with { Entries = manifest.Entries.Select(item => item == entry ? item with { Status = "safety-excluded", Reason = null } : item).ToArray() };
        Assert.Throws<InvalidDataException>(() => CoverageCorpus.Validate(modified, fixtures));
    }

    [Fact]
    public void ScalarComparisonPreservesTypesAndNulls()
    {
        Assert.Equal("null", CoverageCorpus.Scalar(DBNull.Value));
        Assert.Equal("1", CoverageCorpus.Scalar(1));
        Assert.Equal("\"1\"", CoverageCorpus.Scalar("1"));
    }
}
