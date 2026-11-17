using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class QualifiedTrustTests
{
    [Theory]
    [InlineData("USE Sales; SELECT 1;")]
    [InlineData("USE [Sales];\nGO\nSELECT 1;")]
    public void UseIsAContextOperationRatherThanAnUnknownEffect(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "USE" && f.Target == "Sales");
    }

    [Fact]
    public void ARemoteNameDoesNotProveWhichServerOwnsTheLinkedAlias()
    {
        var result = SqlSafetyAnalyzer.Analyze("UPDATE Link.Sales.dbo.T SET a=1;",
            new AnalyzerOptions(new[] { "Link.Sales.dbo.T" }));
        Assert.Equal(Verdict.RealWrite, result.Overall);
    }

    [Fact]
    public void LegacyUnqualifiedTrustNeedsMigrationRatherThanGrantingPermission()
    {
        var result = SqlSafetyAnalyzer.Analyze("EXEC dbo.Cleanup;",
            new AnalyzerOptions(currentDatabase: "Sales", trustedProcedures: new[] { "dbo.Cleanup" }));
        Assert.Equal(Verdict.Unknown, result.Overall);
        Assert.Contains(result.Findings, f => f.Action == "SETTINGS WARNING" && f.Note.Contains("qualification"));
    }

    private static AnalyzerOptions Options(string[]? writes = null, string[]? routines = null, string database = "Sales") =>
        new(writes, currentServer: "Host", currentDatabase: database, trustedProcedures: routines);

    [Theory]
    [InlineData("UPDATE dbo.T SET a=1;")]
    [InlineData("UPDATE [Sales].[dbo].[T] SET a=1;")]
    [InlineData("BEGIN UPDATE dbo.T SET a=1; END")]
    [InlineData("IF 1=1 UPDATE dbo.T SET a=1;")]
    [InlineData("WHILE 1=0 BEGIN UPDATE dbo.T SET a=1; END")]
    [InlineData("BEGIN TRY UPDATE dbo.T SET a=1; END TRY BEGIN CATCH SELECT 1; END CATCH")]
    [InlineData("BEGIN TRAN; UPDATE dbo.T SET a=1; COMMIT;")]
    [InlineData("EXEC(N'UPDATE dbo.T SET a=1;');")]
    [InlineData("EXEC sp_executesql N'UPDATE dbo.T SET a=1;';")]
    [InlineData("SELECT 1;\nGO\nUPDATE dbo.T SET a=1;")]
    [InlineData("UPDATE t SET a=1 FROM dbo.T t;")]
    [InlineData("WITH t AS (SELECT * FROM dbo.T) UPDATE t SET a=1;")]
    public void FullIdentityAuthorizesTheSameTargetThroughWrappers(string sql) =>
        Assert.Equal(Verdict.AllowListed, SqlSafetyAnalyzer.Analyze(sql, Options(new[] { "Host.Sales.dbo.T" })).Overall);

    [Theory]
    [InlineData("Other.Sales.dbo.T")]
    [InlineData("Host.Other.dbo.T")]
    [InlineData("Host.Sales.other.T")]
    [InlineData("Host.Sales.dbo.t")]
    [InlineData("*.Sales.dbo.T")]
    [InlineData("Host.*.dbo.T")]
    public void IdentityMismatchOrBroadContextNeverGrantsTrust(string rule) =>
        Assert.False(SqlSafetyAnalyzer.Analyze("UPDATE dbo.T SET a=1;", Options(new[] { rule })).IsGreen);

    [Fact]
    public void PossibleFailedUseRequiresApprovalForEveryPossibleDatabase()
    {
        var sql = "USE Other;\nGO\nUPDATE dbo.T SET a=1;";
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze(sql, Options(new[] { "Host.Other.dbo.T" })).Overall);
        Assert.Equal(Verdict.AllowListed, SqlSafetyAnalyzer.Analyze(sql, Options(new[] { "Host.Sales.dbo.T", "Host.Other.dbo.T" })).Overall);
        Assert.Equal(Verdict.AllowListed, SqlSafetyAnalyzer.Analyze("USE Other; UPDATE Sales.dbo.T SET a=1;", Options(new[] { "Host.Sales.dbo.T" })).Overall);
    }

    [Fact]
    public void DynamicUseIsScopedButGoPreservesPossibleOuterContexts()
    {
        var options = Options(new[] { "Host.Sales.dbo.T" });
        Assert.Equal(Verdict.AllowListed, SqlSafetyAnalyzer.Analyze("EXEC(N'USE Other; SELECT 1;'); UPDATE dbo.T SET a=1;", options).Overall);
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze("USE Other;\nGO\nUPDATE dbo.T SET a=1;", options).Overall);
    }

    [Theory]
    [InlineData("EXEC dbo.R;", "Host.Sales.dbo.R", Verdict.AllowListed)]
    [InlineData("SELECT dbo.R();", "FUNCTION Host.Sales.dbo.R", Verdict.AllowListed)]
    [InlineData("SELECT * FROM dbo.R();", "FUNCTION Host.Sales.dbo.R", Verdict.AllowListed)]
    [InlineData("SELECT dbo.R();", "Host.Sales.dbo.R", Verdict.Unknown)]
    [InlineData("EXEC dbo.R;", "FUNCTION Host.Sales.dbo.R", Verdict.Unknown)]
    [InlineData("EXEC R;", "Host.Sales.dbo.R", Verdict.Unknown)]
    public void RoutineKindsAreSeparateAndMissingSchemaIsNotGuessed(string sql, string rule, Verdict verdict) =>
        Assert.Equal(verdict, SqlSafetyAnalyzer.Analyze(sql, Options(routines: new[] { rule })).Overall);

    [Fact]
    public void TrustedCallsDoNotAuthorizeDefinitionWritesOrPotentialBodies()
    {
        var result = SqlSafetyAnalyzer.Analyze("CREATE PROCEDURE dbo.R AS BEGIN UPDATE dbo.T SET a=1; END", Options(new[] { "Host.Sales.dbo.T" }, new[] { "Host.Sales.dbo.R" }));
        Assert.Equal(Verdict.RealWrite, result.Overall);
        Assert.Equal(Verdict.RealWrite, Assert.Single(result.CodeBodies).Analysis.Overall);
    }

    [Fact]
    public void DestinationEnvironmentCannotInheritAnOffBypass()
    {
        var rules = new EnvironmentRules("Host | Other | Production | YellowRed | false");
        var global = new GuardSettings(ExecutionMode.Off);
        var options = new AnalyzerOptions(currentServer: "Host", currentDatabase: "Sales", environmentRules: rules, globalSettings: global);
        var result = SqlSafetyAnalyzer.Analyze("USE Other;\nGO\nEXEC dbo.Unavailable;", options);
        Assert.True(global.ShouldWarn(result));
        Assert.True(Assert.Single(result.Findings, f => f.Action == "EXEC").RequiresConfirmation);
    }

    [Theory]
    [InlineData("WHILE 1=1 BEGIN UPDATE dbo.T SET a=1; USE Other; END")]
    [InlineData("again: UPDATE dbo.T SET a=1; USE Other; GOTO again;")]
    public void RepeatedControlFlowCannotReuseTrustFromOnlyTheFirstDatabase(string sql) =>
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze(sql, Options(new[] { "Host.Sales.dbo.T" })).Overall);

    [Fact]
    public void UnknownServerSwitchCannotBypassDestinationPolicies()
    {
        var global = new GuardSettings(ExecutionMode.Off);
        var options = new AnalyzerOptions(sqlCmdMode: true, currentServer: "Host", currentDatabase: "Sales",
            globalSettings: global, environmentRules: new EnvironmentRules("Other | * | Production | YellowRed | false"));
        Assert.True(global.ShouldWarn(SqlSafetyAnalyzer.Analyze(":connect Other\nSELECT 1;", options)));
    }

    [Theory]
    [InlineData("USE Other; SELECT FROM;")]
    [InlineData("UPDATE Linked.Other.dbo.T SET a=1;")]
    public void UnverifiedContextOrBrokenTextCannotInheritOff(string sql)
    {
        var global = new GuardSettings(ExecutionMode.Off);
        var options = new AnalyzerOptions(currentServer: "Host", currentDatabase: "Sales", globalSettings: global,
            environmentRules: new EnvironmentRules("Host | Other | Production | YellowRed | false"));
        Assert.True(global.ShouldWarn(SqlSafetyAnalyzer.Analyze(sql, options)));
    }
}
