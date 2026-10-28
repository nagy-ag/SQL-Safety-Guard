using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CampaignAttributionTests
{
    [Fact]
    public void AnalyzerHostUsesResolvedAliasTarget()
    {
        var host = new AnalyzerHost(typeof(SqlSafetyAnalyzer).Assembly.Location);
        var verdict = host.Analyze("UPDATE orders SET Qty=orders.Qty+1 FROM sales.Orders orders WHERE Id=1;");
        Assert.Equal("RealWrite", verdict.Overall);
        Assert.Contains("sales.Orders", verdict.Targets);
        Assert.DoesNotContain("orders", verdict.Targets);
    }

    [Fact]
    public void AllChangedObjectsMustBeAttributed()
    {
        Assert.Equal("WRONG TARGET", CampaignAttribution.Classify(["dbo.Counter"], ["SgRun_Test.dbo.Counter", "SgRun_Test.app.Accounts"], "SgRun_Test", []));
    }

    [Theory]
    [InlineData("trigger write")]
    [InlineData("temporal/cascade")]
    [InlineData("synonym")]
    [InlineData("single-base view")]
    public void CatalogBackedSecondaryEffectsAreNotWrongTargets(string reason)
    {
        Assert.Equal("INDIRECT EFFECT", CampaignAttribution.Classify(["app.Entry"], ["SgRun_Test.sales.Orders", "SgRun_Audit.audit.Changes"], "SgRun_Test",
            [new("SgRun_Test.app.Entry", "SgRun_Test.sales.Orders", reason), new("SgRun_Test.sales.Orders", "SgRun_Audit.audit.Changes", "trigger write")]));
    }

    [Fact]
    public void IndirectCycleDoesNotHideUnrelatedWrite()
    {
        Assert.Equal("WRONG TARGET", CampaignAttribution.Classify(["app.Entry"], ["SgRun_Test.dbo.Other"], "SgRun_Test",
            [new("SgRun_Test.app.Entry", "SgRun_Test.sales.Orders", "view"), new("SgRun_Test.sales.Orders", "SgRun_Test.app.Entry", "cycle")]));
    }

    [Fact]
    public void DirectTargetRemainsPass()
    {
        Assert.Equal("PASS", CampaignAttribution.Classify(["[dbo].[Counter]"], ["SgRun_Test.[dbo].[Counter]"], "SgRun_Test", []));
    }

    [Fact]
    public void DefaultSchemaWinsOverDboWhenBothObjectsExist()
    {
        Assert.Equal("WRONG TARGET", CampaignAttribution.Classify(["Counter"], ["SgRun_Test.dbo.Counter"], "SgRun_Test", [], "app", ["SgRun_Test.app.Counter", "SgRun_Test.dbo.Counter"]));
        Assert.Equal("PASS", CampaignAttribution.Classify(["Counter"], ["SgRun_Test.app.Counter"], "SgRun_Test", [], "app", ["SgRun_Test.app.Counter", "SgRun_Test.dbo.Counter"]));
    }

    [Fact]
    public void OpaqueBrokerDialogNeedsReview()
    {
        Assert.Equal("UNRESOLVED TARGET", CampaignAttribution.Classify(["@dialog"], ["SgRun_Test.app.WorkQueue"], "SgRun_Test", []));
    }

    [Fact]
    public void ExternalDatabaseInterferenceCannotHideOwnedChanges()
    {
        var before = new CampaignSnapshot(new() { ["owned"] = ["row"] }, new(), new(), Enumerable.Repeat("unchanged", 8).ToArray(), [], []);
        var server = before.Server.ToArray();
        server[5] = "external database recovering";
        var after = before with { Server = server };
        Assert.True(CampaignOracle.OnlyExternalDatabaseChange(before, after));
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, after with { Databases = new() { ["owned"] = ["write"] } }));
        server[1] = "configuration changed";
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, after));
    }

    [Fact]
    public void PristineCheckIncludesTypedRowsMetadataAndWrites()
    {
        var snapshot = new CampaignSnapshot(new() { ["SgRun_Test"] = ["[dbo].[Counter]:int:MQ=="] }, new() { ["Counter"] = 0 }, new() { ["Counter"] = "definition" }, [], [], []);
        Assert.True(CampaignOracle.Equivalent(snapshot, snapshot));
        Assert.False(CampaignOracle.Equivalent(snapshot, snapshot with { Operations = new() { ["Counter"] = 1 } }));
        Assert.False(CampaignOracle.Equivalent(snapshot, snapshot with { Databases = new() { ["SgRun_Test"] = ["changed"] } }));
        Assert.False(CampaignOracle.Equivalent(snapshot, snapshot with { Objects = new() { ["Counter"] = "different" } }));
    }
}
