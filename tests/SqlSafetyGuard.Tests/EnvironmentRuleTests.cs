using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class EnvironmentRuleTests
{
    [Theory]
    [InlineData("prod-sql01", "Orders", "Production", ExecutionMode.YellowRed)]
    [InlineData("PROD-SQL02", "Sandbox", "Production", ExecutionMode.YellowRed)]
    [InlineData("DEV-PC", "Orders", "Development", ExecutionMode.Off)]
    [InlineData("TEST-PC", "Sandbox", "Test", ExecutionMode.Red)]
    [InlineData("TEST-PC", "Orders", "Global", ExecutionMode.YellowRed)]
    [InlineData(null, "Orders", "Global", ExecutionMode.YellowRed)]
    [InlineData("DEV-PC", null, "Global", ExecutionMode.YellowRed)]
    [InlineData("DEV-PC\n", "Orders", "Global", ExecutionMode.YellowRed)]
    public void FirstMatchingRuleWinsAndUnknownContextUsesGlobal(string? server, string? database, string label, ExecutionMode mode)
    {
        var rules = new EnvironmentRules("PROD-SQL* | | Production | YellowRed | true\nPROD-* | | Wrong | Off | false\nDEV-* | | Development | Off | false\nTEST-* | Sand* | Test | Red | false");
        var settings = rules.Resolve(server, database, new GuardSettings());
        Assert.Empty(rules.Warnings);
        Assert.Equal(label, settings.Environment);
        Assert.Equal(mode, settings.Mode);
    }

    [Theory]
    [InlineData("bad")]
    [InlineData("* | | label | bogus | false")]
    [InlineData("* | | label | Red | bogus")]
    [InlineData("* | | label | Red | true | *.*")]
    [InlineData("* | | label | Red | true | | *.proc")]
    [InlineData("* | | label | Red | true | | proc")]
    public void BadRulesAreReportedAndIgnored(string text)
    {
        var rules = new EnvironmentRules(text);
        var global = new GuardSettings();
        Assert.Single(rules.Warnings);
        Assert.Same(global, rules.Resolve("prod", "db", global));
    }

    [Fact]
    public void PerRuleListsAddToGlobalWithoutLeakingToOtherEnvironments()
    {
        var rules = new EnvironmentRules("PROD-* | | Production | YellowRed | true | dbo.Stage*;dbo.Audit | dbo.sp_WhoIsActive;master.dbo.sp_Blitz*");
        var global = new GuardSettings(allowList: new[] { "GlobalTable" }, trustedProcedures: new[] { "dbo.GlobalProc" });
        var settings = rules.Resolve("PROD-A", "Orders", global);
        Assert.Equal(new[] { "GlobalTable", "dbo.Stage*", "dbo.Audit" }, settings.AllowList);
        Assert.Equal(3, settings.TrustedProcedures.Count);
        Assert.True(settings.RequireTypedConfirmation);
        Assert.Same(global, rules.Resolve("OTHER", "Orders", global));
    }

    [Theory]
    [InlineData("dbo.sp_WhoIsActive", null, Verdict.ReadOnly)]
    [InlineData("sp_WhoIsActive", null, Verdict.Unknown)]
    [InlineData("sp_WhoIsActive", "dbo", Verdict.ReadOnly)]
    [InlineData("sp_WhoIsActive", "custom", Verdict.Unknown)]
    [InlineData("other.dbo.sp_WhoIsActive", "dbo", Verdict.Unknown)]
    [InlineData("master.dbo.sp_BlitzFirst", null, Verdict.ReadOnly)]
    [InlineData("linked.master.dbo.sp_BlitzFirst", null, Verdict.Unknown)]
    public void ProcedureTrustRequiresResolvedQualification(string procedure, string? schema, Verdict expected)
    {
        var result = SqlSafetyAnalyzer.Analyze("EXEC " + procedure,
            new AnalyzerOptions(currentDatabase: "Orders", defaultSchema: schema,
                trustedProcedures: new[] { "dbo.sp_WhoIsActive", "master.dbo.sp_Blitz*" }));
        Assert.Equal(expected, result.Overall);
        if (expected == Verdict.ReadOnly) Assert.Contains(result.Findings, finding => finding.Note.Contains("trusted procedure"));
    }

    [Theory]
    [InlineData("USE Other; DELETE dbo.Safe", Verdict.RealWrite)]
    [InlineData(":connect Other\nDELETE dbo.Safe", Verdict.RealWrite)]
    [InlineData("USE Other; EXEC dbo.SafeProc", Verdict.Unknown)]
    [InlineData("SELECT 'USE Other'; EXEC dbo.SafeProc", Verdict.ReadOnly)]
    [InlineData("EXEC(N'USE Other; DELETE dbo.Safe')", Verdict.RealWrite)]
    public void TargetSwitchesDisableContextualTrust(string sql, Verdict expected)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql, new AnalyzerOptions(new[] { "dbo.Safe" }, currentDatabase: "Orders",
            trustedProcedures: new[] { "dbo.SafeProc" }));
        Assert.Equal(expected, result.Overall);
        if (expected != Verdict.ReadOnly) Assert.Contains(result.Findings, finding => finding.Action == "TARGET SWITCH");
    }

    [Theory]
    [InlineData(ExecutionMode.Off, "DELETE dbo.Orders", false)]
    [InlineData(ExecutionMode.Red, "EXEC dbo.opaque", false)]
    [InlineData(ExecutionMode.Red, "DELETE dbo.Orders", true)]
    [InlineData(ExecutionMode.YellowRed, "EXEC dbo.opaque", true)]
    [InlineData(ExecutionMode.YellowRed, "SELECT 1", false)]
    public void ModesControlWarnings(ExecutionMode mode, string sql, bool warn) =>
        Assert.Equal(warn, new GuardSettings(mode).ShouldWarn(SqlSafetyAnalyzer.Analyze(sql)));

    [Theory]
    [InlineData("DELETE dbo.Orders", true)]
    [InlineData("UPDATE dbo.Orders SET Id=1", true)]
    [InlineData("TRUNCATE TABLE dbo.Orders", true)]
    [InlineData("DROP TABLE dbo.Orders", true)]
    [InlineData("DROP DATABASE Orders", true)]
    [InlineData("DROP INDEX IX_Orders ON dbo.Orders", true)]
    [InlineData("DROP PROCEDURE dbo.Opaque", true)]
    [InlineData("DROP PROCEDURE #temp", false)]
    [InlineData("EXEC dbo.Opaque", false)]
    [InlineData("DELETE dbo.Orders WHERE Id=1", false)]
    [InlineData("DELETE #temp", false)]
    public void TypedConfirmationAppliesOnlyToRedWholeTargetFindings(string sql, bool required) =>
        Assert.Equal(required, new GuardSettings(requireTypedConfirmation: true).NeedsTypedConfirmation(SqlSafetyAnalyzer.Analyze(sql)));

    [Theory]
    [InlineData("Orders", "Orders", true)]
    [InlineData("Orders", "orders", false)]
    [InlineData(null, "", false)]
    [InlineData("", "", false)]
    public void TypedDatabaseNameMustBeKnownAndExact(string? database, string entered, bool matches) =>
        Assert.Equal(matches, GuardSettings.ConfirmationMatches(database, entered));

    [Theory]
    [InlineData("DELETE dbo.Orders")]
    [InlineData("EXEC dbo.Opaque")]
    public void PresentationOrLoggingFailuresCannotAuthorizeWarning(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        bool confirmed = false;
        Assert.True(GuardDecision.ShouldCancel(new GuardSettings(), result, () => throw new Exception("results"),
            () => { confirmed = true; return false; }, exception => throw exception));
        Assert.True(confirmed);
        Assert.True(GuardDecision.ShouldCancel(new GuardSettings(), result, () => { },
            () => throw new Exception("dialog"), exception => { }));
    }

    [Theory]
    [InlineData(ExecutionMode.YellowRed, "DELETE dbo.Orders", true)]
    [InlineData(ExecutionMode.YellowRed, "EXEC dbo.Opaque", true)]
    [InlineData(ExecutionMode.Red, "EXEC dbo.Opaque", false)]
    [InlineData(ExecutionMode.Off, "DELETE dbo.Orders", false)]
    public void LateWaitOrLoggingFailurePreservesComputedDecision(ExecutionMode mode, string sql, bool cancel) =>
        Assert.Equal(cancel, GuardDecision.CancelAfterFailure(new GuardSettings(mode), SqlSafetyAnalyzer.Analyze(sql)));

    [Fact]
    public void FailureBeforeClassificationFailsOpen() =>
        Assert.False(GuardDecision.CancelAfterFailure(new GuardSettings(), null));

    [Theory]
    [InlineData(ExecutionMode.Off, false)]
    [InlineData(ExecutionMode.Red, false)]
    [InlineData(ExecutionMode.YellowRed, true)]
    public void OversizedScriptsFollowExplicitModePolicy(ExecutionMode mode, bool warn)
    {
        var result = SqlSafetyAnalyzer.Analyze(new string(' ', SqlSafetyAnalyzer.MaxTextLength + 1));
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Note.Contains("too large to analyze") && finding.Note.Contains("MB"));
        Assert.Equal(warn, new GuardSettings(mode).ShouldWarn(result));
    }

    [Fact]
    public void MalformedSettingsAreVisibleInAnalysis()
    {
        var rules = new EnvironmentRules("bad rule");
        var result = SqlSafetyAnalyzer.Analyze("SELECT 1", new AnalyzerOptions(configurationWarnings: rules.Warnings));
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Action == "SETTINGS WARNING");
    }

    [Fact]
    public void FederationSwitchInvalidatesTargetContext()
    {
        var result = SqlSafetyAnalyzer.Analyze("USE FEDERATION ROOT WITH RESET");
        Assert.Empty(result.ParseErrors);
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, finding => finding.Action == "TARGET SWITCH");
    }
}
