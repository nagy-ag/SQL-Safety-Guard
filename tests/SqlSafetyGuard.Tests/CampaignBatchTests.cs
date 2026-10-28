using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CampaignBatchTests
{
    [Fact]
    public void SubstitutesVariablesInsideMultilineStringsAndComments()
    {
        var script = SqlCmdLite.Split(":setvar value 42\nSELECT N'first\n$(value)';\n/* first\n$(value) */", true);
        Assert.Null(script.NotRunnable);
        Assert.Equal("SELECT N'first\n42';\n/* first\n42 */\n", Assert.Single(script.Batches).Sql);
    }

    [Theory]
    [InlineData(true, "SELECT 42;\n")]
    [InlineData(false, ":setvar value 42\nSELECT $(value);\n")]
    public void RespectsExplicitSqlCmdMode(bool enabled, string expected)
    {
        var script = SqlCmdLite.Split(":setvar value 42\nSELECT $(value);", enabled);
        Assert.Null(script.NotRunnable);
        Assert.Equal(expected, Assert.Single(script.Batches).Sql);
    }

    [Theory]
    [InlineData(":r secret.sql")]
    [InlineData(":connect other")]
    [InlineData("!! whoami")]
    public void RefusesExternalSqlCmdActions(string sql)
    {
        Assert.NotNull(SqlCmdLite.Split(sql, true).NotRunnable);
    }
    [Theory]
    [InlineData("SELECT N'line1\nGO\nUPDATE dbo.Target SET Value=1';\nGO\nSELECT 2;")]
    [InlineData("SELECT N'escaped ''quote\nGO\nend';\nGO\nSELECT 2;")]
    [InlineData("/* outer /* inner */\nGO\n*/ SELECT 1;\nGO\nSELECT 2;")]
    [InlineData("SELECT [name\nGO\nend];\nGO\nSELECT 2;")]
    [InlineData("SELECT \"name\nGO\nend\";\nGO\nSELECT 2;")]
    public void SplitsOnlyUnquotedTopLevelGo(string sql)
    {
        var script = SqlCmdLite.Split(sql, false);
        Assert.Null(script.NotRunnable);
        Assert.Equal(2, script.Batches.Count);
        Assert.Contains("\nGO\n", script.Batches[0].Sql);
        Assert.Equal("SELECT 2;\n", script.Batches[1].Sql);
    }
}
