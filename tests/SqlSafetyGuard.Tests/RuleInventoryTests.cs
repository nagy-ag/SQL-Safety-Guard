using System.Reflection;
using System.Text.Json;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlSafetyGuard.Analyzer;
using Xunit.Abstractions;

namespace SqlSafetyGuard.Tests;

public class RuleInventoryTests(ITestOutputHelper output)
{
    internal static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));

    internal static JsonDocument Inventory() => JsonDocument.Parse(
        File.ReadAllText(Path.Combine(Root, "docs/verification/rules.json")));

    [Fact]
    public void ReportUncoveredRuleIds()
    {
        using var inventory = Inventory();
        using var mapping = JsonDocument.Parse(File.ReadAllText(Path.Combine(Root, "docs/verification/rule-test-map.json")));
        var rules = inventory.RootElement.GetProperty("rules").EnumerateArray().ToArray();
        var identifiers = rules.Select(rule => rule.GetProperty("id").GetString()!).ToHashSet();
        Assert.Equal(rules.Length, identifiers.Count);
        var methods = typeof(RuleInventoryTests).Assembly.GetTypes().SelectMany(type => type.GetMethods())
            .Where(method => method.GetCustomAttributes().Any(attribute => attribute is FactAttribute || attribute is TheoryAttribute))
            .ToArray();
        var covered = new HashSet<string>();
        foreach (var mappingEntry in mapping.RootElement.EnumerateObject())
        {
            Assert.Contains(mappingEntry.Name, identifiers);
            foreach (var reference in mappingEntry.Value.EnumerateArray())
            {
                string text = reference.GetString()!;
                if (text.Contains('('))
                {
                    string methodName = text[(text.LastIndexOf('(') + 1)..].TrimEnd(')');
                    Assert.Contains(methods, method => method.Name == methodName);
                }
                else
                {
                    Assert.Contains(methods, method => method.DeclaringType!.FullName + "." + method.Name == text);
                }
            }
            Assert.NotEmpty(mappingEntry.Value.EnumerateArray());
            covered.Add(mappingEntry.Name);
        }
        var uncovered = identifiers.Except(covered).Order().ToArray();
        output.WriteLine($"Rule references: {covered.Count}/{identifiers.Count}; uncovered: {uncovered.Length}. REPORT ONLY, not branch or server proof.");
        foreach (string identifier in uncovered)
            output.WriteLine("UNCOVERED " + identifier);
    }

    public static IEnumerable<object[]> TableRules()
    {
        using var inventory = Inventory();
        foreach (var rule in inventory.RootElement.GetProperty("rules").EnumerateArray())
        {
            string category = rule.GetProperty("category").GetString()!;
            if (category is not ("safe-procedure" or "writing-procedure" or "safe-dbcc" or "changing-dbcc" or "unknown-dbcc" or "set-option" or "set-family"))
                continue;
            yield return [rule.GetProperty("id").GetString()!, category,
                rule.GetProperty("member").GetString()!,
                rule.TryGetProperty("sql", out var sql) ? sql.GetString()! : ""];
        }
    }

    [Theory]
    [MemberData(nameof(TableRules))]
    public void TableDecisionMatchesInventory(string ruleId, string category, string member, string sqlOption)
    {
        output.WriteLine("RULE " + ruleId);
        if (category == "set-option" && member == "DisableDefCnstChk")
        {
            var classifier = typeof(SqlSafetyAnalyzer).Assembly.GetType("SqlSafetyGuard.Analyzer.Classifier")!;
            var statement = new PredicateSetStatement { Options = SetOptions.DisableDefCnstChk, IsOn = true };
            Assert.True((bool)classifier.GetMethod("IsReadOnlyStatement")!.Invoke(null, [statement])!);
            var unsupported = SqlSafetyAnalyzer.Analyze($"SET {sqlOption} ON;");
            Assert.NotEmpty(unsupported.ParseErrors);
            Assert.False(unsupported.IsGreen);
            output.WriteLine("NOT EXECUTABLE in current parser: legacy enum only; AST classification tested separately.");
            return;
        }
        if (category is "safe-dbcc" or "changing-dbcc" or "unknown-dbcc")
        {
            var classifier = typeof(SqlSafetyAnalyzer).Assembly.GetType("SqlSafetyGuard.Analyzer.Classifier")!;
            var statement = new DbccStatement { Command = Enum.Parse<DbccCommand>(member) };
            object?[] arguments = [statement, null];
            var actual = (Verdict)classifier.GetMethod("ClassifyDbcc")!.Invoke(null, arguments)!;
            Assert.Equal(category == "safe-dbcc" ? Verdict.ReadOnly : category == "changing-dbcc" ? Verdict.RealWrite : Verdict.Unknown, actual);
            return;
        }
        string sql = category == "set-option" ? $"SET {sqlOption} ON;" : category == "set-family" ? $"SET {sqlOption};" : $"EXEC sys.{member};";
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.Empty(result.ParseErrors);
        Assert.Equal(category == "writing-procedure" ? Verdict.RealWrite : Verdict.ReadOnly, result.Overall);
    }
}
