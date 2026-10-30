using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Options;

namespace SqlSafetyGuard.Tests;

public class SettingsTransferTests
{
    private static readonly DateTimeOffset Timestamp = DateTimeOffset.Parse("2026-10-30T08:00:00Z");
    private static string Export(SettingsSnapshot settings) => SettingsTransfer.Export(settings, "1.0.0.0", Timestamp);
    private static SettingsImport Import(SettingsSnapshot before, SettingsSnapshot after) => SettingsTransfer.PrepareImport(Export(after), before);
    private static readonly SettingsSnapshot Defaults = new();

    [Fact]
    public void RoundTripPreservesEverySettingAndListLineIncludingCommentsAndBlanks()
    {
        var settings = new SettingsSnapshot(ExecutionMode.Red, false, "-- note\r\ndbo.Staging_*\r\n\r\n", true,
            "dbo.Reader\nmaster.dbo.sp_Blitz*", "-- first match\nprod* | Sales | Production | YellowRed | false | dbo.Stage | dbo.Reader\n");
        var json = Export(settings);
        var root = JObject.Parse(json);
        Assert.Equal(1, (int)root["formatVersion"]!);
        Assert.Equal("1.0.0.0", (string?)root["extensionVersion"]);
        Assert.Equal(Timestamp, (DateTimeOffset)root["exportedAt"]!);
        Assert.IsType<JArray>(root["settings"]!["allowList"]);
        var imported = SettingsTransfer.PrepareImport(json, Defaults).Proposed;
        Assert.Empty(SettingsTransfer.Diff(settings, imported));
        Assert.Equal(6, Import(Defaults, settings).Changes.Count);
        Assert.Contains(Import(Defaults, settings).Changes, change => change.Name == "Trusted procedures");
    }

    [Theory]
    [InlineData("true")]
    [InlineData("\"not even a boolean\"")]
    [InlineData("{\"malicious\":true}")]
    public void LegacyTypedConfirmationIsNeverSerializedOrConsumed(string ignoredValue)
    {
        var root = JObject.Parse(Export(Defaults));
        Assert.DoesNotContain("RequireTypedConfirmation", root.ToString(), StringComparison.OrdinalIgnoreCase);
        root["settings"]!["requireTypedConfirmation"] = JToken.Parse(ignoredValue);
        root["RequireTypedConfirmation"] = JToken.Parse(ignoredValue);
        var import = SettingsTransfer.PrepareImport(root.ToString(), Defaults);
        Assert.Empty(import.Changes);
        Assert.Empty(import.Warnings);
        Assert.Contains("settings.requireTypedConfirmation", import.IgnoredKeys);
        Assert.DoesNotContain("requireTypedConfirmation", Export(import.Proposed), StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<object[]> InvalidFiles()
    {
        yield return new object[] { "not json", "JSON" };
        yield return new object[] { "[]", "JSON" };
        yield return new object[] { "{}", "formatVersion" };
        yield return new object[] { Export(Defaults) + "{}", "JSON" };
        foreach (var mutation in new (string path, JToken? value, string message)[]
        {
            ("formatVersion", 2, "Unsupported"), ("formatVersion", 0, "Unsupported"), ("formatVersion", "1", "Integer"),
            ("formatVersion", 1.0, "Integer"), ("formatVersion", null, "formatVersion"),
            ("extensionVersion", false, "extensionVersion"), ("exportedAt", "yesterday", "timestamp"),
            ("settings", "bad", "Object"), ("settings.guardMode", "Unknown", "guardMode"),
            ("settings.guardMode", "0", "guardMode"), ("settings.guardMode", 0, "String"),
            ("settings.showServerAndDatabase", "true", "Boolean"), ("settings.tempdbIsTemp", 1, "Boolean"),
            ("settings.allowList", "dbo.X", "Array"), ("settings.trustedProcedures", new JArray(2), "strings"),
            ("settings.allowList", new JArray("dbo.X\ndbo.Y"), "one line"),
            ("settings.allowList", new JArray("*"), "allow-list"),
            ("settings.allowList", new JArray("[unclosed"), "allow-list"),
            ("settings.trustedProcedures", new JArray("Reader"), "trusted procedure"),
            ("settings.trustedProcedures", new JArray("*.dbo.Reader"), "trusted procedure"),
            ("settings.environmentRules", new JArray("bad rule"), "Environment rule"),
            ("settings.environmentRules", new JArray("* | | X | Missing | false"), "Environment rule"),
            ("settings.environmentRules", new JArray("* | | X | Red | false | *"), "allow-list"),
            ("settings.environmentRules", new JArray("* | | X | Red | false | | proc"), "trusted procedure"),
        })
        {
            var root = JObject.Parse(Export(Defaults));
            var old = root.SelectToken(mutation.path)!;
            if (mutation.value == null) ((JProperty)old.Parent!).Remove();
            else old.Replace(mutation.value);
            yield return new object[] { root.ToString(), mutation.message };
        }
        yield return new object[] { Export(Defaults).Replace("\"formatVersion\": 1", "\"formatVersion\": 1, \"formatVersion\": 1"), "JSON" };
    }

    [Theory]
    [MemberData(nameof(InvalidFiles))]
    public void InvalidImportCannotChangeCurrentSettings(string json, string message)
    {
        var current = new SettingsSnapshot(ExecutionMode.Red, false, "dbo.Existing", true, "dbo.Reader");
        string original = Export(current);
        var error = Assert.Throws<FormatException>(() => SettingsTransfer.PrepareImport(json, current));
        Assert.Contains(message, error.Message);
        Assert.Equal(original, Export(current));
    }

    [Fact]
    public void UnknownKeysAreIgnoredAndNamedInPreview()
    {
        var root = JObject.Parse(Export(Defaults));
        root["future"] = new JObject();
        root["settings"]!["futureToggle"] = true;
        var import = SettingsTransfer.PrepareImport(root.ToString(), Defaults);
        Assert.Equal(new[] { "future", "settings.futureToggle" }, import.IgnoredKeys);
        Assert.Empty(import.Changes);
    }

    [Fact]
    public void ApplyingValidatedImportWritesAllSettingsAndPersistsOnce()
    {
        var import = Import(Defaults, new SettingsSnapshot(ExecutionMode.Off, false, "dbo.Stage", true, "dbo.Reader"));
        var current = Defaults;
        int saves = 0;
        SettingsTransfer.Apply(import, () => current, settings => current = settings, () => saves++);
        Assert.Same(import.Proposed, current);
        Assert.Equal(1, saves);
    }

    [Fact]
    public void StorageFailureRestoresAllPreviousValues()
    {
        var import = Import(Defaults, new SettingsSnapshot(ExecutionMode.Off, false, "dbo.Stage", true));
        var current = Defaults;
        int saves = 0;
        Assert.Throws<IOException>(() => SettingsTransfer.Apply(import, () => current, settings => current = settings, () =>
        {
            if (++saves == 1) throw new IOException("simulated storage failure");
        }));
        Assert.Same(Defaults, current);
        Assert.Equal(2, saves);
    }

    [Fact]
    public void StalePreviewDoesNotOverwriteNewerSettings()
    {
        var import = Import(Defaults, new SettingsSnapshot(ExecutionMode.Off));
        var changedElsewhere = new SettingsSnapshot(showServerAndDatabase: false);
        Assert.Throws<InvalidOperationException>(() => SettingsTransfer.Apply(import, () => changedElsewhere,
            _ => throw new Exception("must not assign"), () => throw new Exception("must not save")));
    }

    [Fact]
    public void OversizedFileIsRejectedBeforeParsing()
    {
        Assert.Throws<FormatException>(() => SettingsTransfer.PrepareImport(new string(' ', SettingsTransfer.MaximumFileCharacters + 1), Defaults));
    }

    [Theory]
    [InlineData(ExecutionMode.Red)]
    [InlineData(ExecutionMode.Off)]
    public void LowerGlobalGuardModeIsHighlighted(ExecutionMode mode)
    {
        Assert.Contains(Import(Defaults, new SettingsSnapshot(mode)).Warnings, warning => warning.Contains("F5 guard is lowered"));
    }

    [Fact]
    public void AllGlobalGreenWideningChangesAreHighlighted()
    {
        var import = Import(Defaults, new SettingsSnapshot(allowList: "dbo.One\ndbo.Two\ndbo.Stage*", tempdbIsTemp: true,
            trustedProcedures: "dbo.Reader\ndbo.Fetch*"));
        Assert.Equal(3, import.Warnings.Count);
        Assert.Contains(import.Warnings, text => text.Contains("allow-lists 3") && text.Contains("writes matching them will be green"));
        Assert.Contains(import.Warnings, text => text.Contains("trusts 2") && text.Contains("user asserted"));
        Assert.Contains(import.Warnings, text => text.Contains("Permanent tempdb"));
    }

    [Theory]
    [InlineData("* | | Dev | Off | false | |", "F5 guard is lowered")]
    [InlineData("prod* | Sales | Prod | Red | false | dbo.Stage |", "allow-lists 1")]
    [InlineData("prod? | Sales* | Prod | YellowRed | false | | dbo.Reader", "trusts 1")]
    public void EnvironmentWideningIsHighlighted(string rule, string warning)
    {
        Assert.Contains(Import(Defaults, new SettingsSnapshot(environmentRules: rule)).Warnings, text => text.Contains("Environment") && text.Contains(warning));
    }

    [Fact]
    public void RemovingOrReorderingRestrictiveOverrideCannotSilentlyExposeWeakerGlobalOrRule()
    {
        var before = new SettingsSnapshot(ExecutionMode.Off, environmentRules: "prod* | | Prod | YellowRed | false\n* | | Other | Off | false | dbo.Stage | dbo.Reader");
        var removed = Import(before, new SettingsSnapshot(ExecutionMode.Off));
        Assert.Contains(removed.Warnings, text => text.Contains("lowered"));
        var reordered = Import(before, new SettingsSnapshot(ExecutionMode.Off,
            environmentRules: "* | | Other | Off | false | dbo.Stage | dbo.Reader\nprod* | | Prod | YellowRed | false"));
        Assert.Contains(reordered.Warnings, text => text.Contains("lowered"));
        Assert.Contains(reordered.Warnings, text => text.Contains("allow-lists"));
        Assert.Contains(reordered.Warnings, text => text.Contains("trusts"));
    }

    [Fact]
    public void BroadeningSelectorOrChangingDatabaseScopeIsDetected()
    {
        var before = new SettingsSnapshot(environmentRules: "prod1 | Sales | X | Off | false | dbo.Stage | dbo.Reader");
        foreach (string rule in new[] { "prod* | Sales | X | Off | false | dbo.Stage | dbo.Reader", "prod1 | | X | Off | false | dbo.Stage | dbo.Reader" })
        {
            var import = Import(before, new SettingsSnapshot(environmentRules: rule));
            Assert.Contains(import.Warnings, text => text.Contains("lowered"));
            Assert.Contains(import.Warnings, text => text.Contains("allow-lists"));
        }
    }

    [Fact]
    public void NonWideningChangesDoNotProduceWarnings()
    {
        var before = new SettingsSnapshot(ExecutionMode.Off, true, "dbo.Stage*\ndbo.Remove", true, "dbo.Reader*",
            "prod* | | Old | Off | false | dbo.Local* | dbo.Fetch*");
        var after = new SettingsSnapshot(ExecutionMode.Red, false, "[DBO].[Stage_One]", false, "dbo.ReaderOne",
            "prod1 | Sales | Renamed | Red | true | dbo.LocalOne | dbo.FetchOne");
        Assert.Empty(Import(before, after).Warnings);
        Assert.Empty(Import(after, after).Warnings);
        Assert.Empty(Import(Defaults, new SettingsSnapshot(showServerAndDatabase: false, allowList: "-- comment")).Warnings);
    }

    [Fact]
    public void RemovingEnvironmentTrustAndStrengtheningModesIsNotWidening()
    {
        var before = new SettingsSnapshot(environmentRules: "* | | Dev | Red | false | dbo.Stage | dbo.Reader");
        Assert.Empty(Import(before, Defaults).Warnings);
    }

    [Theory]
    [InlineData("dbo.A*;dbo.B*", "dbo.A?;dbo.B?", false)]
    [InlineData("dbo.A*;dbo.?*", "dbo.*", false)]
    [InlineData("Stage*", "dbo.StageOne", false)]
    [InlineData("*.dbo.Stage*", "dbo.StageOne", false)]
    [InlineData("dbo.Stage*", "Sales.dbo.StageOne", true)]
    [InlineData("Sales.dbo.Stage*", "dbo.StageOne", true)]
    [InlineData("dbo.Stage*", "StageOne", true)]
    [InlineData("dbo.Stage*", "Linked.Sales.dbo.StageOne", true)]
    public void PatternInclusionRespectsUnionsAndQualificationBoundaries(string oldPatterns, string newPatterns, bool widens)
    {
        var warnings = Import(new SettingsSnapshot(allowList: oldPatterns), new SettingsSnapshot(allowList: newPatterns)).Warnings;
        Assert.Equal(widens, warnings.Count > 0);
    }

    [Fact]
    public void ConnectionRepresentativesAlwaysHaveNonWhitespaceNames()
    {
        Assert.All(GlobRegions.Representatives(new[] { "A B", "A", "B", "*" }), name => Assert.False(string.IsNullOrWhiteSpace(name)));
    }

    [Fact]
    public void EnvironmentPatternCoverageUsesItsCurrentDatabaseAndChecksEveryDatabaseRegion()
    {
        var before = new SettingsSnapshot(allowList: "Sales.dbo.Stage*");
        Assert.Empty(Import(before, new SettingsSnapshot(allowList: before.AllowList,
            environmentRules: "* | Sales | Sales | YellowRed | false | dbo.StageOne")).Warnings);
        Assert.Contains(Import(before, new SettingsSnapshot(allowList: before.AllowList,
            environmentRules: "* | Sales* | Sales | YellowRed | false | dbo.StageOne")).Warnings, warning => warning.Contains("allow-lists"));
    }

    [Fact]
    public void CoverageNeverMissesAnExpansionInSampledActualAllowListMatches()
    {
        var patterns = new[] { "dbo.A*", "dbo.?*", "dbo.A?", "dbo.*B", "dbo.K*", "dbo.K*", "dbo.ı*", "dbo.I*", "dbo.ſ*", "dbo.S*", "Sales.dbo.A*", "A*" };
        var names = new[] { "A", "AB", "B", "AAA", "AC", "K", "K", "k", "ı", "I", "i", "ſ", "S", "s" };
        foreach (string before in patterns)
            foreach (string after in patterns)
            {
                if (Import(new SettingsSnapshot(allowList: before), new SettingsSnapshot(allowList: after)).Warnings.Count > 0) continue;
                var oldList = new AllowList(new[] { before });
                var newList = new AllowList(new[] { after });
                foreach (string database in new[] { "Sales", "Other" })
                    foreach (string schema in new[] { "dbo", "custom", null! })
                        foreach (string name in names)
                            Assert.False(newList.Matches(null, database, schema, name, "Sales") && !oldList.Matches(null, database, schema, name, "Sales"),
                                $"Missed expansion {before} -> {after} for {database}.{schema}.{name}");
            }
    }

    [Fact]
    public void ChangedRulePriorityWithOverlappingSelectorsIsComparedInEveryRegion()
    {
        var before = new SettingsSnapshot(environmentRules: "prod? | Sales* | Strict | YellowRed | false\n*prod* | * | Loose | Off | false");
        var after = new SettingsSnapshot(environmentRules: "*prod* | * | Loose | Off | false\nprod? | Sales* | Strict | YellowRed | false");
        Assert.Contains(Import(before, after).Warnings, text => text.Contains("lowered"));
    }

    [Fact]
    public void PreviewUsesThemedButtonsWithCancelAsDefaultAndWarningsBeforeChanges()
    {
        var repo = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        var dialog = XDocument.Load(Path.Combine(repo, "src/SqlSafetyGuard.Vsix/UI/SettingsImportDialog.xaml"));
        XNamespace p = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        Assert.Empty(dialog.Descendants(p + "TextBox"));
        var buttons = dialog.Descendants(p + "Button").ToArray();
        Assert.Equal(2, buttons.Length);
        Assert.All(buttons, b => Assert.Contains("ThemedDialogButtonStyleKey", (string?)b.Attribute("Style")));
        Assert.Contains(buttons, b => (string?)b.Attribute(x + "Name") == "CancelButton" && (string?)b.Attribute("IsDefault") == "True");
        var names = dialog.Descendants().Select(e => (string?)e.Attribute(x + "Name")).ToArray();
        Assert.True(Array.IndexOf(names, "WarningSection") < Array.IndexOf(names, "ChangesHeading"));
        Assert.Contains("Binding Before", dialog.ToString());
        Assert.Contains("Binding After", dialog.ToString());
    }

    [Fact]
    public void ToolsMenuOwnsAnalyzeAndSettingsWhileTransferActionsLiveOnNativeSettingsPage()
    {
        var repo = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        XNamespace ns = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable";
        var commands = XDocument.Load(Path.Combine(repo, "src/SqlSafetyGuard.Vsix/SqlSafetyGuardPackage.vsct"));
        var buttons = commands.Descendants(ns + "Button").ToArray();
        var menuButtons = buttons.Where(button => (string?)button.Element(ns + "Parent")?.Attribute("id") == "ToolsMenuGroup");
        Assert.Equal(new[] { "Analyze Query", "Settings..." },
            menuButtons.Select(button => (string?)button.Element(ns + "Strings")?.Element(ns + "ButtonText")));
        var menu = Assert.Single(commands.Descendants(ns + "Menu"));
        Assert.Equal("SQL Safety Guard", (string?)menu.Element(ns + "Strings")?.Element(ns + "ButtonText"));
        Assert.All(buttons.Where(button => new[] { "cmdidExportSettings", "cmdidImportSettings" }.Contains((string?)button.Attribute("id"))),
            button => Assert.Null(button.Element(ns + "Parent")));
        var registration = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine(repo, "src/SqlSafetyGuard.Vsix/Options/registration.json")));
        Assert.DoesNotContain("docs/", registration.ToString());
        var actions = (Newtonsoft.Json.Linq.JArray)registration["categories"]!["sqlSafetyGuard"]!["commands"]!;
        Assert.Equal(new[] { "Import Settings...", "Export Settings..." }, actions.Select(action => (string?)action["vsct"]!["text"]));
        Assert.Equal(new[] { 0x102, 0x101 }, actions.Select(action => (int)action["vsct"]!["id"]!));
        var adapter = File.ReadAllText(Path.Combine(repo, "src/SqlSafetyGuard.Vsix/Commands/SettingsCommands.cs"));
        Assert.DoesNotContain("RequireTypedConfirmation", adapter);
    }

    [Fact]
    public void NativeSettingsPreserveLegacyStoreKeysDefaultsAndBidirectionalMigration()
    {
        var repo = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        var registration = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine(repo, "src/SqlSafetyGuard.Vsix/Options/registration.json")));
        var properties = (Newtonsoft.Json.Linq.JObject)registration["properties"]!;
        var legacy = new Dictionary<string, string>
        {
            ["guardMode"] = "GuardMode", ["showConnection"] = "ShowServerAndDatabase", ["allowList"] = "AllowList",
            ["trustedProcedures"] = "TrustedProcedures", ["environmentRules"] = "EnvironmentRules", ["tempdbIsTemp"] = "TempdbIsTemp"
        };
        Assert.Equal(legacy.Count, properties.Count);
        foreach (var pair in legacy)
        {
            var setting = properties["sqlSafetyGuard." + pair.Key]!;
            var migration = setting["migration"]![pair.Key == "guardMode" ? "enumIntegerToString" : "pass"]!;
            Assert.Equal("SettingsManager", (string?)migration["input"]!["store"]);
            Assert.Equal("SqlSafetyGuard.Options.GeneralOptionsPage." + pair.Value, (string?)migration["input"]!["path"]);
            Assert.Equal("full", (string?)migration["mode"]);
            Assert.NotEqual(true, (bool?)setting["transientScopesSupported"]);
        }
        Assert.Equal("YellowRed", (string?)properties["sqlSafetyGuard.guardMode"]!["default"]);
        var modeMap = (JArray)properties["sqlSafetyGuard.guardMode"]!["migration"]!["enumIntegerToString"]!["map"]!;
        Assert.Equal(new[] { "Off", "Red", "YellowRed" }, modeMap.Select(entry => (string?)entry["result"]));
        Assert.Equal(new[] { 0, 1, 2 }, modeMap.Select(entry => (int)entry["match"]!));
        Assert.False((bool)properties["sqlSafetyGuard.tempdbIsTemp"]!["default"]!);
        Assert.True((bool)properties["sqlSafetyGuard.showConnection"]!["default"]!);
        Assert.All(new[] { "allowList", "trustedProcedures", "environmentRules" }, name =>
        {
            Assert.Equal("", (string?)properties["sqlSafetyGuard." + name]!["default"]);
            Assert.Single((Newtonsoft.Json.Linq.JArray)properties["sqlSafetyGuard." + name]!["commands"]!);
        });
    }
}
