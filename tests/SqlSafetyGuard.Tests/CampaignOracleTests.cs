using SqlSafetyGuard.ServerHarness;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.Tests;

public class CampaignOracleTests
{
    [Fact]
    public void PermissionSnapshotRequiresAnExistingNonCloneGrantee()
    {
        var query = Assert.Single(CampaignOracle.ServerQueries, query => query.Contains("FROM sys.server_permissions"));
        Assert.Contains("INNER JOIN sys.server_principals grantee ON grantee.principal_id=permissions.grantee_principal_id", query);
        Assert.Contains("WHERE (grantee.name NOT LIKE 'SgRun[_]%')", query);
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

    [Fact]
    public void ServerSnapshotQueriesParseForSharedAndIsolatedServers()
    {
        foreach (var isolated in new[] { false, true })
            foreach (var query in CampaignOracle.ServerQueriesFor(["SgRun_owned_Core"], ["SgRun_owned"], isolated))
            {
                new TSql180Parser(true).Parse(new StringReader(query), out var errors);
                Assert.Empty(errors);
            }
    }

    [Fact]
    public void SharedServerFiltersOtherClonesButIncludesOwnedAndNonCloneState()
    {
        var queries = CampaignOracle.ServerQueriesFor(["SgRun_owned_Core"], ["SgRun_owned", "SgRun_owned_low_user"]);
        Assert.Contains("(name NOT LIKE 'SgRun[_]%' OR name IN (N'SgRun_owned',N'SgRun_owned_low_user'))", queries[0]);
        Assert.Contains("INNER JOIN sys.server_principals role ON role.principal_id=memberships.role_principal_id", queries[3]);
        Assert.Contains("INNER JOIN sys.server_principals member ON member.principal_id=memberships.member_principal_id", queries[3]);
        foreach (var principal in new[] { "role", "member", "grantee" })
            Assert.Contains(queries, query => query.Contains($"({principal}.name NOT LIKE 'SgRun[_]%' OR {principal}.name IN (N'SgRun_owned',N'SgRun_owned_low_user'))"));
        Assert.Contains("(name NOT LIKE 'SgRun[_]%' OR name IN (N'SgRun_owned_Core'))", queries[5]);
        Assert.DoesNotContain(queries, query => query.Contains("SgRun_other"));
        Assert.DoesNotContain("WHERE", queries[1]);
        Assert.DoesNotContain("WHERE", queries[2]);
        Assert.DoesNotContain("WHERE", queries[6]);
        Assert.DoesNotContain("WHERE", queries[7]);
    }

    [Fact]
    public void IsolatedServerDoesNotExcludeEvenCloneNamedCaseChanges()
    {
        var queries = CampaignOracle.ServerQueriesFor(["SgRun_owned_Core"], ["SgRun_owned"], isolated: true);
        Assert.DoesNotContain(queries, query => query.Contains("NOT LIKE") || query.Contains(" IN ("));
    }

    [Theory]
    [InlineData(0, "[{\"name\":\"CaseLogin\"}]")]
    [InlineData(1, "[{\"name\":\"xp_cmdshell\",\"value\":1}]")]
    [InlineData(2, "[{\"name\":\"CaseLinkedServer\"}]")]
    [InlineData(3, "[{\"role_principal_id\":3,\"member_principal_id\":1000}]")]
    [InlineData(4, "[{\"class\":100,\"grantee_principal_id\":1000,\"type\":\"COSQ\"}]")]
    [InlineData(5, "[{\"name\":\"SgRun_owned_Core\",\"is_read_only\":true}]")]
    [InlineData(6, "[{\"name\":\"CaseEndpoint\"}]")]
    [InlineData(7, "[{\"name\":\"CaseSession\"}]")]
    public void GenuineServerChangesRemainObservedWithOrWithoutExternalDatabaseChanges(int index, string change)
    {
        var before = Snapshot();
        var session = new CampaignSession(0, 0, "runner", "runner", "runner", "owned");
        foreach (var external in new[] { false, true })
        {
            var server = before.Server.ToArray();
            server[index] = change;
            if (external && index != 5) server[5] = "[{\"name\":\"SgExt_other\"}]";
            var after = before with { Server = server };
            Assert.False(CampaignOracle.OnlyExternalDatabaseChange(before, after));
            var observation = CampaignOracle.Compare(before, after, session, session);
            Assert.True(observation.Changed);
            Assert.True(observation.ServerChanged);
            Assert.Contains("<server-state>", observation.ModifiedObjects);
        }
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
