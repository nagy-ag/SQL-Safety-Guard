using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.UI;

namespace SqlSafetyGuard.Tests;

public class PanePresentationTests
{
    [Theory]
    [InlineData("USE [Sandbox]; SELECT 1;")]
    [InlineData("EXEC(N'USE [Sandbox]; SELECT 1;');")]
    public void ContextHasItsOwnDisplayCountAndFilterWhileKeepingItsAnalyzerVerdict(string sql)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        var context = Assert.Single(result.Findings, f => f.IsContextChange);
        Assert.Equal(Verdict.ReadOnly, context.Verdict);
        Assert.Equal(Verdict.ReadOnly, result.Overall);
        var counts = new PaneDisplayCounts(result.Findings);
        Assert.Equal(1, counts.Context);
        Assert.Equal(1, counts.Safe);
        Assert.Equal(2, counts.Total);
        Assert.Equal(counts.Total, counts.Writes + counts.Unknown + counts.Safe + counts.Context);
        for (int mask = 0; mask < 64; mask++)
        {
            Assert.Equal((mask & (int)PaneVerdictFilter.Context) != 0, PaneFilterRules.Includes((PaneVerdictFilter)mask, context));
            Assert.Equal((mask & (int)PaneVerdictFilter.Safe) != 0,
                PaneFilterRules.Includes((PaneVerdictFilter)mask, result.Findings.Single(f => !f.IsContextChange)));
        }
    }

    public static IEnumerable<object[]> VerdictCombinations()
    {
        for (int combination = 1; combination < 8; combination++)
            yield return new object[] { combination };
    }

    [Theory]
    [MemberData(nameof(VerdictCombinations))]
    public void FilteringPreservesOverallRiskAndOriginalOperationIdentity(int combination)
    {
        var source = new List<Finding>();
        if ((combination & 1) != 0) source.Add(Operation(Verdict.RealWrite));
        if ((combination & 2) != 0) source.Add(Operation(Verdict.Unknown));
        if ((combination & 4) != 0) source.Add(Operation(Verdict.ReadOnly));
        var original = source.ToArray();
        var counts = new PaneVerdictCounts(source.Select(f => f.Verdict));

        foreach (var filter in Enum.GetValues<PaneVerdictFilter>())
        {
            var visible = PaneFilterRules.Select(source, filter, f => f.Verdict);
            var expected = source.Where(f => f.Verdict == Verdict.RealWrite && (filter & PaneVerdictFilter.Writes) != 0 ||
                f.Verdict == Verdict.Unknown && (filter & PaneVerdictFilter.Unknown) != 0 ||
                f.Verdict == Verdict.ReadOnly && (filter & PaneVerdictFilter.Safe) != 0).ToArray();
            Assert.Equal(expected, visible);
            Assert.Equal(original, source);
            Assert.Equal(original.Length, counts.Total);
            Assert.Equal((combination & 1) != 0 ? 1 : 0, counts.Writes);
            Assert.Equal((combination & 2) != 0 ? 1 : 0, counts.Unknown);
            Assert.Equal((combination & 4) != 0 ? 1 : 0, counts.Safe);
        }
    }

    [Fact]
    public void EveryToggleCombinationUsesTheUnionOfRiskColorsWithoutPromotingScopeColorsToVerdicts()
    {
        var source = new[] { Operation(Verdict.RealWrite), Operation(Verdict.Unknown), Operation(Verdict.ReadOnly), Operation(Verdict.TempOnly), Operation(Verdict.AllowListed) };
        for (int mask = 0; mask < 64; mask++)
        {
            var visible = PaneFilterRules.Select(source, (PaneVerdictFilter)mask, f => f.Verdict);
            Assert.Equal(((mask & 1) != 0 ? 1 : 0) + ((mask & 2) != 0 ? 1 : 0) + ((mask & 4) != 0 ? 3 : 0), visible.Count);
            Assert.Equal(5, source.Length);
        }
    }

    [Fact]
    public void SameLineAndOverlappingDynamicSqlOperationsAreNotLost()
    {
        var first = Operation(Verdict.RealWrite);
        var second = Operation(Verdict.RealWrite);
        var uncertainty = Operation(Verdict.Unknown);
        var source = new[] { first, second, uncertainty };
        var writes = PaneFilterRules.Select(source, PaneVerdictFilter.Writes, f => f.Verdict);
        Assert.Equal(2, writes.Count);
        Assert.Same(first, writes[0]);
        Assert.Same(second, writes[1]);
        Assert.Single(PaneFilterRules.Select(source, PaneVerdictFilter.Unknown, f => f.Verdict));
        Assert.Equal(3, new PaneVerdictCounts(source.Select(f => f.Verdict)).Total);
    }

    [Fact]
    public void AllGreenKindsRemainVisibleWithoutChangingTheirMeaning()
    {
        var source = new[] { Operation(Verdict.ReadOnly), Operation(Verdict.TempOnly), Operation(Verdict.AllowListed) };
        Assert.Equal(source, PaneFilterRules.Select(source, PaneVerdictFilter.Safe, f => f.Verdict));
        Assert.Equal(3, new PaneVerdictCounts(source.Select(f => f.Verdict)).Safe);
        Assert.Empty(PaneFilterRules.Select(source, PaneVerdictFilter.Writes, f => f.Verdict));
    }

    private static Finding Operation(Verdict verdict) => new(1, 1, 0, 12, "UPDATE", "dbo.Example", null!, verdict, "example");

    [Fact]
    public void DecodedBlocksKeepTheirOperationsAndDoNotMergeSeparateExecs()
    {
        var result = SqlSafetyAnalyzer.Analyze("EXEC(N'UPDATE dbo.T SET a=1 WHERE id=1; UPDATE dbo.T SET b=2 WHERE id=2; SELECT 1;'); EXEC(N'SELECT 2;'); EXEC(@runtime);");
        Assert.Equal(Verdict.RealWrite, result.Overall);
        var blocks = result.Findings.Where(PaneOperationPresentation.IsDynamic)
            .GroupBy(PaneOperationPresentation.DynamicKey).ToList();
        Assert.Equal(3, blocks.Count);
        Assert.Equal(3, blocks[0].Count());
        Assert.Equal(2, blocks[0].Count(f => f.Verdict.IsRed()));
        Assert.Single(blocks[0], f => f.Verdict.IsGreen());
        Assert.All(blocks[0], f => Assert.True(PaneOperationPresentation.IsDecodedDynamic(f)));
        Assert.Equal(Verdict.Unknown, Assert.Single(blocks[2]).Verdict);
        Assert.False(PaneOperationPresentation.IsDecodedDynamic(blocks[2].Single()));
        Assert.Equal(result.Findings.Count, blocks.Sum(block => block.Count()));
    }
}
