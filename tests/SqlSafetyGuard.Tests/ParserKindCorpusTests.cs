using System.Text.Json;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.ServerHarness;
using Verdict = SqlSafetyGuard.Analyzer.Verdict;

namespace SqlSafetyGuard.Tests;

public class ParserKindCorpusTests
{
    public static IEnumerable<object[]> DialectCases()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Coverage", "dialect-fixtures.json")));
        foreach (var fixture in document.RootElement.GetProperty("fixtures").EnumerateArray())
            yield return new object[] { fixture.GetProperty("statementType").GetString()!, fixture.GetProperty("dialect").GetString()!, fixture.GetProperty("sql").GetString()!, fixture.GetProperty("expectedVerdict").GetString()!, fixture.GetProperty("defaultParserErrors").GetInt32() };
    }

    [Theory]
    [MemberData(nameof(DialectCases))]
    public void AlternateDialectHasExactKindAndDefaultAnalyzerNeverClaimsGreen(string statementType, string dialect, string sql, string expectedVerdict, int defaultParserErrors)
    {
        TSqlParser parser = dialect switch
        {
            "80" => new TSql80Parser(true),
            "FabricDW" => new TSqlFabricDWParser(true),
            _ => throw new InvalidDataException("Unreviewed dialect: " + dialect)
        };
        Assert.Contains(statementType, CoverageCorpus.StatementTypes(sql, parser));
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Equal(defaultParserErrors, result.ParseErrors.Count);
        Assert.Equal(expectedVerdict, result.Overall.ToString());
        Assert.Contains(result.Overall, new[] { Verdict.RealWrite, Verdict.Unknown });
    }

    public static IEnumerable<object[]> ParserCases()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Coverage", "parser-fixtures.json")));
        foreach (var fixture in document.RootElement.GetProperty("fixtures").EnumerateArray())
            yield return new object[] { fixture.GetProperty("statementType").GetString()!, fixture.GetProperty("sql").GetString()!, fixture.GetProperty("expectedVerdict").GetString()! };
    }

    [Theory]
    [MemberData(nameof(ParserCases))]
    public void RealSqlParsesToDeclaredKindAndMatchesReviewedVerdict(string statementType, string sql, string expectedVerdict)
    {
        Assert.Contains(statementType, CoverageCorpus.StatementTypes(sql));
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(expectedVerdict, result.Overall.ToString());
        Assert.NotEmpty(result.Findings);
    }

    [Fact]
    public void ParserFixtureKindsAreUniqueAndKnown()
    {
        var kinds = ParserCases().Select(row => (string)row[0]).ToArray();
        Assert.Equal(kinds.Length, kinds.Distinct().Count());
        var (manifest, _) = CoverageCorpus.Load(Path.Combine(AppContext.BaseDirectory, "Coverage"));
        Assert.All(kinds, kind => Assert.Contains(manifest.Entries, entry => entry.StatementType == kind));
    }
}
