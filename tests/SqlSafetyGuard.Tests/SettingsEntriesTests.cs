using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Options;

namespace SqlSafetyGuard.Tests;

/// <summary>The structured settings editors are views of the stored text; they must never change what matches.</summary>
public class SettingsEntriesTests
{
    private const string Tables = "-- Demo safe tables\ndbo.SgDemo_Staging_*\ndbo.SgDemo_ImportBuffer;Sandbox.dbo.Test_*\n# trailing comment";
    private const string Rules =
        "-- Local playground\n*SQLEXPRESS | SgPlay_* | Playground | YellowRed | false | dbo.SgDemo_Playground_* | dbo.usp_SgDemo_PlaygroundRead\n" +
        "DEMO-PROD-SQL* | | PROD demo | yellowred | true\n" +
        "DEMO-TEST-SQL* | Sandbox* | Test demo | Red | false | | dbo.usp_Read_*";

    [Fact]
    public void PatternsParseIntoPartsWithNotes()
    {
        var entries = PatternText.Parse(Tables, procedures: false, out var trailing);
        Assert.Equal(new[] { "dbo.SgDemo_Staging_*", "dbo.SgDemo_ImportBuffer", "Sandbox.dbo.Test_*" }, entries.Select(entry => entry.Pattern));
        Assert.Equal("Demo safe tables", entries[0].Note);
        Assert.Equal(("", "", "dbo", "SgDemo_Staging_*"), (entries[0].Server, entries[0].Database, entries[0].Schema, entries[0].Name));
        Assert.Equal(("", "Sandbox", "dbo", "Test_*"), (entries[2].Server, entries[2].Database, entries[2].Schema, entries[2].Name));
        Assert.Equal(new[] { "# trailing comment" }, trailing);
        Assert.All(entries, entry => Assert.Null(entry.Problem));
    }

    [Fact]
    public void UneditedEntriesKeepTheirExactText()
    {
        const string text = "[dbo] . [Odd.Name]\n\"sales\".Orders";
        var entries = PatternText.Parse(text, false, out var trailing);
        Assert.Equal("[dbo] . [Odd.Name]\n\"sales\".Orders", PatternText.Compose(entries, trailing));
        Assert.Equal("Odd.Name", entries[0].Name);
    }

    [Fact]
    public void ComposedPatternsMatchExactlyWhatThePartsSay()
    {
        var entry = new PatternEntry(procedures: false) { Schema = "dbo", Name = "Odd.Name" };
        Assert.Equal("dbo.[Odd.Name]", entry.Pattern);
        var list = new AllowList(new[] { entry.Pattern });
        Assert.True(list.Matches(null, null, "dbo", "Odd.Name"));
        Assert.False(list.Matches(null, null, "dbo", "Odd"));

        var comment = new PatternEntry(false) { Name = "--Looks_like_a_comment" };
        Assert.Equal("[--Looks_like_a_comment]", comment.Pattern);
        Assert.Single(new AllowList(new[] { comment.Pattern }).Patterns);

        var full = new PatternEntry(false) { Server = "Reporting", Database = "Sandbox", Schema = "dbo", Name = "Staging_*" };
        Assert.Equal("Reporting.Sandbox.dbo.Staging_*", full.Pattern);
        Assert.True(new AllowList(new[] { full.Pattern }).Matches("Reporting", "Sandbox", "dbo", "Staging_Orders"));
    }

    [Theory]
    [InlineData(false, "", "", "", "", "Enter a table name.")]
    [InlineData(false, "", "Sandbox", "", "T", "A database also needs a schema, for example dbo.")]
    [InlineData(false, "Srv", "", "dbo", "T", "A server also needs a database and a schema.")]
    [InlineData(false, "", "", "*", "*", "Only wildcards: this pattern would be ignored. Name at least one part.")]
    [InlineData(true, "", "", "", "usp_Read", "Enter a schema, for example dbo. Procedures must be schema-qualified.")]
    [InlineData(true, "", "", "db*", "usp_Read", "Wildcards are allowed only in the procedure name.")]
    [InlineData(false, "", "", "dbo", "A;B", "Names can't contain ; or line breaks.")]
    public void InvalidPartsExplainTheProblem(bool procedures, string server, string database, string schema, string name, string problem)
    {
        var entry = new PatternEntry(procedures) { Server = server, Database = database, Schema = schema, Name = name };
        Assert.Equal(problem, entry.Problem);
    }

    [Fact]
    public void ExistingInvalidPatternsAreFlaggedNotHidden()
    {
        var entries = PatternText.Parse("*.*\nusp_NoSchema", procedures: true, out _);
        Assert.All(entries, entry => Assert.NotNull(entry.Problem));
        Assert.Equal("*.*\nusp_NoSchema", PatternText.Compose(entries));
        Assert.Equal((2, 2), PatternText.Summarize("*.*\nusp_NoSchema", procedures: true));
    }

    [Fact]
    public void NotesCannotSplitIntoPatterns()
    {
        var entry = new PatternEntry(false) { Schema = "dbo", Name = "T", Note = "first; second" };
        Assert.Equal("A note can't contain ; or line breaks.", entry.Problem);
    }

    [Theory]
    [InlineData("dbo.SgDemo_Staging_*", "dbo.SgDemo_Staging_Orders", "✓ Matches: writes to it would be green.")]
    [InlineData("dbo.SgDemo_Staging_*", "SgDemo_Staging_Orders", "✗ Doesn't match: write the schema too, for example dbo.SgDemo_Staging_Orders.")]
    [InlineData("dbo.SgDemo_Staging_*", "Other.dbo.SgDemo_Staging_Orders", "✗ Doesn't match: a pattern without a database matches only the current database.")]
    [InlineData("Sandbox.dbo.T_*", "dbo.T_1", "✓ Matches only when the window's current database is Sandbox.")]
    [InlineData("Staging_*", "sales.Staging_1", "✓ Matches: writes to it would be green.")]
    [InlineData("dbo.Staging_*", "dbo.Other", "✗ Doesn't match.")]
    public void TryANameUsesTheAnalyzerMatcher(string pattern, string candidate, string expected)
    {
        var entry = PatternText.Parse(pattern, false, out _).Single();
        Assert.Equal(expected, PatternText.Try(entry, candidate));
    }

    [Fact]
    public void TryAcceptsAnExecStatementForProcedures()
    {
        var entry = PatternText.Parse("dbo.usp_Read_*", true, out _).Single();
        Assert.Equal("✓ Matches: calls to it would be green.", PatternText.Try(entry, "EXEC dbo.usp_Read_Orders @id = 1;"));
        Assert.Equal("✗ Doesn't match: write the schema too, for example dbo.usp_Read_Orders.", PatternText.Try(entry, "EXEC usp_Read_Orders"));
    }

    [Fact]
    public void RulesParseIntoFieldsAndKeepTheirLines()
    {
        var rules = RuleText.Parse(Rules, out var trailing);
        Assert.Empty(trailing);
        Assert.Equal(3, rules.Count);
        Assert.Equal(("*SQLEXPRESS", "SgPlay_*", "Playground", "YellowRed"), (rules[0].Server, rules[0].Database, rules[0].Label, rules[0].Mode));
        Assert.Equal("Local playground", rules[0].Note);
        Assert.Equal("dbo.SgDemo_Playground_*", rules[0].SafeTables);
        Assert.Equal("dbo.usp_SgDemo_PlaygroundRead", rules[0].TrustedProcedures);
        Assert.True(rules[1].IsProduction);
        Assert.Equal("YellowRed", rules[1].Mode);
        Assert.Equal("1 · Playground", rules[0].Title);
        Assert.Equal("DEMO-TEST-SQL* / Sandbox* · red only", rules[2].Subtitle);
        Assert.Equal(Rules, RuleText.Compose(rules));
        Assert.All(rules, rule => Assert.Null(rule.Problem));
    }

    [Fact]
    public void EditedRulesComposeTheStoredFormat()
    {
        var rule = RuleEntry.New();
        rule.Server = "TEST-SQL*";
        rule.Label = "Test";
        rule.Mode = "Red";
        Assert.Equal("TEST-SQL* |  | Test | Red | false", rule.Line);
        rule.TrustedProcedures = "dbo.usp_A\r\ndbo.usp_B_*";
        Assert.Equal("TEST-SQL* |  | Test | Red | false |  | dbo.usp_A;dbo.usp_B_*", rule.Line);
        rule.SafeTables = "dbo.Stage_*";
        Assert.Equal("TEST-SQL* |  | Test | Red | false | dbo.Stage_* | dbo.usp_A;dbo.usp_B_*", rule.Line);
        Assert.Empty(new EnvironmentRules(rule.Line).Warnings);
        var resolved = new EnvironmentRules(rule.Line).Resolve("TEST-SQL1", "Any", new GuardSettings(ExecutionMode.YellowRed, "Global", false, null, null, "global"));
        Assert.Equal(ExecutionMode.Red, resolved.Mode);
    }

    [Theory]
    [InlineData("", "Label", "Enter a server name or pattern.")]
    [InlineData("SRV", "", "Enter a label.")]
    [InlineData("SRV|X", "Label", "Fields can't contain |.")]
    public void InvalidRulesExplainTheProblem(string server, string label, string problem)
    {
        var rule = RuleEntry.New();
        rule.Server = server;
        rule.Label = label;
        Assert.Equal(problem, rule.Problem);
    }

    [Fact]
    public void RuleListsAreValidatedLikeTheGlobalLists()
    {
        var rule = RuleEntry.New();
        rule.Server = "SRV";
        rule.Label = "Label";
        rule.TrustedProcedures = "usp_NoSchema";
        Assert.StartsWith("Extra trusted procedures: ", rule.Problem);
        var unreadable = RuleText.Parse("not a rule", out _).Single();
        Assert.False(unreadable.Readable);
        Assert.NotNull(unreadable.Problem);
        Assert.Equal("not a rule", RuleText.Compose(new[] { unreadable }));
        Assert.Equal((1, 1), RuleText.Summarize("-- only a note\nnot a rule"));
    }

    [Fact]
    public void EditorOutputSurvivesExportAndImport()
    {
        var tables = PatternText.Parse(Tables, false, out var trailing);
        tables.Add(new PatternEntry(false) { Schema = "dbo", Name = "Odd.Name", Note = "added in the editor" });
        var rules = RuleText.Parse(Rules, out var ruleTrailing);
        var added = RuleEntry.New();
        added.Server = "NEW-SQL*";
        added.Label = "New";
        added.SafeTables = "dbo.New_*";
        rules.Add(added);
        var procedures = new[] { new PatternEntry(true) { Schema = "dbo", Name = "usp_Read_*" } };

        var proposed = new SettingsSnapshot(ExecutionMode.YellowRed, true, PatternText.Compose(tables, trailing), false,
            PatternText.Compose(procedures), RuleText.Compose(rules, ruleTrailing));
        var json = SettingsTransfer.Export(proposed, "test", DateTimeOffset.UnixEpoch);
        var imported = SettingsTransfer.PrepareImport(json, new SettingsSnapshot()).Proposed;

        Assert.Equal(proposed.AllowList, imported.AllowList);
        Assert.Equal(proposed.TrustedProcedures, imported.TrustedProcedures);
        Assert.Equal(proposed.EnvironmentRules, imported.EnvironmentRules);
        Assert.Equal(tables.Select(entry => entry.Pattern), PatternText.Parse(imported.AllowList, false, out _).Select(entry => entry.Pattern));
        Assert.Equal(rules.Select(rule => rule.Line), RuleText.Parse(imported.EnvironmentRules, out _).Select(rule => rule.Line));
        Assert.Equal("added in the editor", PatternText.Parse(imported.AllowList, false, out _).Last().Note);
    }

    [Fact]
    public void ReorderedEntriesKeepTheirMeaning()
    {
        var before = new AllowList(AllowList.SplitLines(Tables));
        var entries = PatternText.Parse(Tables, false, out var trailing);
        entries.Reverse();
        var after = new AllowList(AllowList.SplitLines(PatternText.Compose(entries, trailing)));
        Assert.Equal(before.Patterns.OrderBy(value => value), after.Patterns.OrderBy(value => value));
        Assert.Empty(after.Warnings);
    }

    [Fact]
    public void SummariesCountEntriesAndProblems()
    {
        Assert.Equal((3, 0), PatternText.Summarize(Tables, procedures: false));
        Assert.Equal((0, 0), PatternText.Summarize("", procedures: false));
        Assert.Equal((3, 0), RuleText.Summarize(Rules));
        Assert.Equal((0, 0), RuleText.Summarize("-- just a comment"));
    }
}
