using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class GuardExecutionContractTests
{
    [Theory]
    [MemberData(nameof(Decisions))]
    public void UnverifiedTargetAlwaysRequiresApproval(ExecutionMode mode, string sql, bool approved)
    {
        int confirmations = 0;
        bool cancelled = GuardDecision.ShouldCancel(new GuardSettings(mode), SqlSafetyAnalyzer.Analyze(sql), () => { },
            () => { confirmations++; return approved; }, _ => Assert.Fail("Unexpected failure"), executionTargetVerified: false);
        Assert.Equal(1, confirmations);
        Assert.Equal(!approved, cancelled);
    }

    [Theory]
    [InlineData(ExecutionMode.Off)]
    [InlineData(ExecutionMode.Red)]
    [InlineData(ExecutionMode.YellowRed)]
    public void UnknownTargetAndGreenTextDoNotBypassBrokenConfirmation(ExecutionMode mode)
    {
        int reports = 0;
        bool cancelled = GuardDecision.ShouldCancel(new GuardSettings(mode), SqlSafetyAnalyzer.Analyze(""),
            () => throw new InvalidOperationException("presentation"),
            () => throw new InvalidOperationException("confirmation"), _ => reports++, executionTargetVerified: false);
        Assert.True(cancelled);
        Assert.Equal(2, reports);
    }

    public static IEnumerable<object[]> Decisions()
    {
        foreach (var mode in Enum.GetValues<ExecutionMode>())
        foreach (var sql in new[] { "SELECT 1;", "INSERT #temp VALUES (1);", "EXEC dbo.Opaque;", "DELETE dbo.Orders;" })
        foreach (bool approved in new[] { false, true })
            yield return new object[] { mode, sql, approved };
    }

    [Theory]
    [MemberData(nameof(Decisions))]
    public void SharedProductionDecisionControlsOneExecution(ExecutionMode mode, string sql, bool approved)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        var settings = new GuardSettings(mode, mode == ExecutionMode.Off ? "Development" : "Production");
        int presentations = 0;
        int confirmations = 0;
        int executions = 0;
        bool cancelled = GuardDecision.ShouldCancel(settings, result, () => presentations++,
            () => { confirmations++; return approved; }, _ => Assert.Fail("Unexpected adapter failure"));
        if (!cancelled) executions++;
        bool warn = mode != ExecutionMode.Off && (mode == ExecutionMode.Red ? result.Overall == Verdict.RealWrite : !result.IsGreen);
        Assert.Equal(1, presentations);
        Assert.Equal(warn ? 1 : 0, confirmations);
        Assert.Equal(warn && !approved, cancelled);
        Assert.Equal(warn && !approved ? 0 : 1, executions);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void PresentationFailureNeverBypassesARequiredDecision(bool approved, bool loggingFails)
    {
        int confirmations = 0;
        int executions = 0;
        int reports = 0;
        bool cancelled = GuardDecision.ShouldCancel(new GuardSettings(), SqlSafetyAnalyzer.Analyze("DELETE dbo.Orders;"),
            () => throw new InvalidOperationException("presentation"),
            () => { confirmations++; return approved; },
            _ => { reports++; if (loggingFails) throw new InvalidOperationException("logging"); });
        if (!cancelled) executions++;
        Assert.Equal(1, confirmations);
        Assert.Equal(1, reports);
        Assert.Equal(approved ? 1 : 0, executions);
    }

    [Theory]
    [InlineData("DELETE dbo.Orders;")]
    [InlineData("EXEC dbo.Opaque;")]
    public void ConfirmationFailureAlwaysPreventsExecution(string sql)
    {
        int executions = 0;
        bool cancelled = GuardDecision.ShouldCancel(new GuardSettings(), SqlSafetyAnalyzer.Analyze(sql), () => { },
            () => throw new InvalidOperationException("dialog"), _ => throw new InvalidOperationException("logging"));
        if (!cancelled) executions++;
        Assert.True(cancelled);
        Assert.Equal(0, executions);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void LegacyTypedPolicyUsesOnlyButtonApproval(bool approved, int expectedExecutions)
    {
        var settings = new GuardSettings(requireTypedConfirmation: true);
        var result = SqlSafetyAnalyzer.Analyze("DELETE dbo.Orders;");
        int executions = 0;
        bool cancelled = GuardDecision.ShouldCancel(settings, result, () => { }, () => approved, _ => { });
        if (!cancelled) executions++;
        Assert.Equal(expectedExecutions, executions);
    }


}
