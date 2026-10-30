using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CampaignOracleTests
{
    [Fact]
    public void PermissionSnapshotRequiresAnExistingNonCloneGrantee()
    {
        var query = Assert.Single(CampaignOracle.ServerQueries, query => query.Contains("FROM sys.server_permissions"));
        Assert.Contains("INNER JOIN sys.server_principals grantee ON grantee.principal_id=permissions.grantee_principal_id", query);
        Assert.Contains("WHERE grantee.name NOT LIKE 'SgRun[_]%'", query);
        Assert.DoesNotContain("NOT IN", query);
        Assert.DoesNotContain(CampaignOracle.ServerQueries, query => query.Contains("SELECT principal_id FROM sys.server_principals"));
    }

    [Fact]
    public void ForeignRecoveryStateIsExternalButOwnedOrConfigurationChangesAreNot()
    {
        var before = Snapshot("[{\"name\":\"Foreign\",\"state\":0,\"is_read_only\":false}]");
        Assert.True(CampaignOracle.OnlyExternalDatabaseChange(before, Snapshot("[{\"name\":\"Foreign\",\"state\":2,\"is_read_only\":false}]")));
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, Snapshot("[{\"name\":\"Foreign\",\"state\":2,\"is_read_only\":true}]")));
        foreach (var name in new[] { "SgPlay_Core", "SgRun_owned", "master", "tempdb" })
            Assert.False(CampaignOracle.OnlyExternalDatabaseChange(Snapshot("[{\"name\":\"" + name + "\",\"state\":0}]"), Snapshot("[{\"name\":\"" + name + "\",\"state\":2}]")));
    }

    private static CampaignSnapshot Snapshot(string databases = "[]", string sessions = "[]") =>
        new([], [], [], ["[]", "[]", "[]", "[]", "[]", databases, "[]", sessions], [], []);

    [Fact]
    public void AttributesOnlyExplicitExtensionDatabaseAndSessionMetadata()
    {
        var before = Snapshot();
        var after = Snapshot("[{\"name\":\"SgExt_owned_by_other_agent\",\"state\":0}]", "[{\"name\":\"SgExt_events\"}]");
        Assert.True(CampaignOracle.OnlyExternalDatabaseChange(before, after));
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, Snapshot("[{\"name\":\"Unrelated\"}]")));
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, Snapshot(sessions: "[{\"name\":\"OtherSession\"}]")));
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, after with { Tempdb = ["write"] }));
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, after with { Operations = new() { ["owned"] = 1 } }));
        var server = after.Server.ToArray();
        server[1] = "configuration changed";
        Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, after with { Server = server }));
    }
}
