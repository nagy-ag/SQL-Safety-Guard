using System.Reflection;
using System.Text.Json;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class AstOnlyKindTests
{
    public static IEnumerable<object[]> Cases()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Coverage", "ast-only-fixtures.json")));
        foreach (var fixture in document.RootElement.GetProperty("fixtures").EnumerateArray())
            yield return new object[] { fixture.GetProperty("statementType").GetString()!, fixture.GetProperty("expectedVerdict").GetString()! };
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ConstructedAstUsesReviewedFallbackWithoutClaimingSqlExecution(string statementType, string expectedVerdict)
    {
        var type = typeof(TSqlStatement).Assembly.GetType("Microsoft.SqlServer.TransactSql.ScriptDom." + statementType)!;
        var statement = (TSqlStatement)Activator.CreateInstance(type)!;
        var findings = new List<Finding>();
        var walker = new StatementWalker(new AnalyzerOptions(), findings, new List<SqlParseError>(), new List<SkippedRegion>());
        typeof(StatementWalker).GetMethod("WalkStatement", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(walker, new object[] { statement, Scope.ForText(string.Empty) });
        Assert.Single(findings);
        Assert.Equal(expectedVerdict, findings[0].Verdict.ToString());
    }

    [Fact]
    public void EveryConcreteKindHasExactlyOneStaticEvidenceCategory()
    {
        var parserKinds = ParserKindCorpusTests.ParserCases().Select(row => (string)row[0]);
        var dialectKinds = ParserKindCorpusTests.DialectCases().Select(row => (string)row[0]);
        var astKinds = Cases().Select(row => (string)row[0]);
        var all = parserKinds.Concat(dialectKinds).Concat(astKinds).ToArray();
        var (manifest, _) = CoverageCorpus.Load(Path.Combine(AppContext.BaseDirectory, "Coverage"));
        Assert.Equal(334, all.Length);
        Assert.Equal(all.Length, all.Distinct().Count());
        Assert.Equal(manifest.Entries.Select(entry => entry.StatementType).Order(), all.Order());
    }
}
