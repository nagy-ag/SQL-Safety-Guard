using System.Globalization;
using System.Xml.Linq;

namespace SqlSafetyGuard.Tests;

public class GuardUiContractTests
{
    [Fact]
    public void ConfirmationUsesCollapsedPassiveScrollableLedgers()
    {
        var repository = TestRepository.Root;
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/ConfirmExecutionDialog.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        Assert.Equal(2, document.Descendants(presentation + "Expander").Count());
        Assert.Contains(document.Descendants(presentation + "Setter"), element =>
            (string?)element.Attribute("Property") == "IsExpanded" && (string?)element.Attribute("Value") == "False");
        Assert.Equal(2, document.Descendants(presentation + "ItemsControl").Count());
        Assert.Empty(document.Descendants(presentation + "ListBox"));
        Assert.Empty(document.Descendants(presentation + "TextBox"));
        Assert.DoesNotContain(document.Descendants().Attributes(), attribute => attribute.Name.LocalName.StartsWith("Mouse"));
        Assert.All(document.Descendants(presentation + "Expander"), section =>
        {
            var scroll = Assert.Single(section.Elements(presentation + "ScrollViewer"));
            Assert.True(double.TryParse((string?)scroll.Attribute("MaxHeight"), NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var maxHeight) && double.IsFinite(maxHeight) && maxHeight > 0,
                "Each ledger needs a finite positive height so long findings remain scrollable.");
            Assert.Equal("Auto", (string?)scroll.Attribute("VerticalScrollBarVisibility"));
        });
        Assert.All(document.Descendants(presentation + "Button"), button =>
            Assert.Empty(button.Ancestors(presentation + "ScrollViewer")));
        var text = document.ToString();
        Assert.Contains("CHANGES TO PERMANENT OBJECTS", text);
        Assert.Contains("CAN'T SEE INSIDE", text);
        var warning = Assert.Single(document.Descendants(presentation + "TextBlock"), element =>
            (string?)element.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}Name") == "TargetWarningText");
        Assert.Contains("ThemedDialogColors.WindowPanelTextBrushKey", (string?)warning.Attribute("Foreground"));
        Assert.DoesNotContain("Binding Note", text);
        Assert.DoesNotContain("Binding Finding", text);
    }

    [Fact]
    public void CollapsedPaneHasPassiveBadgesAndOneExpandTarget()
    {
        var repository = TestRepository.Root;
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/SafetyPaneControl.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace markup = "http://schemas.microsoft.com/winfx/2006/xaml";
        var strip = Assert.Single(document.Descendants(presentation + "Border"),
            element => (string?)element.Attribute(markup + "Name") == "CollapsedStrip");

        Assert.Equal("Transparent", (string?)strip.Attribute("Background"));
        Assert.Equal("OnToggleClicked", (string?)strip.Attribute("MouseLeftButtonUp"));
        Assert.Empty(strip.Descendants(presentation + "Button"));
        Assert.DoesNotContain(strip.Descendants(presentation + "TextBlock"),
            element => element.Attribute("Text") != null);
        Assert.DoesNotContain(strip.Descendants().Attributes(),
            attribute => attribute.Name.LocalName.StartsWith("Mouse", StringComparison.Ordinal));
        Assert.Single(strip.Descendants(presentation + "StackPanel"), element =>
            (string?)element.Attribute(markup + "Name") == "CollapsedCounts");
        Assert.DoesNotContain(strip.Descendants(), element =>
            (string?)element.Attribute(markup + "Name") == "CollapsedGlyph");
    }

    [Theory]
    [InlineData("ConfirmExecutionDialog.xaml")]
    [InlineData("GuardFailureDialog.xaml")]
    public void ConfirmationButtonsHaveSafeKeyboardDefaults(string file)
    {
        var repository = TestRepository.Root;
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/", file));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace markup = "http://schemas.microsoft.com/winfx/2006/xaml";
        var buttons = document.Descendants(presentation + "Button").ToList();
        Assert.Empty(document.Descendants(presentation + "TextBox"));
        var run = Assert.Single(buttons, button => (string?)button.Attribute(markup + "Name") == "RunButton");
        var cancel = Assert.Single(buttons, button => (string?)button.Attribute(markup + "Name") == "CancelButton");
        Assert.Equal("Run anyway", (string?)run.Attribute("Content"));
        Assert.NotEqual("True", (string?)run.Attribute("IsDefault"));
        Assert.Empty(run.Descendants(presentation + "AccessText"));
        Assert.Equal("True", (string?)cancel.Attribute("IsDefault"));
        Assert.Equal("True", (string?)cancel.Attribute("IsCancel"));
    }

    [Fact]
    public void ProcedureBodyFindingsAreLabeledAsDeferredAndNavigateWithButtons()
    {
        var document = XDocument.Load(Path.Combine(TestRepository.Root, "src/SqlSafetyGuard.Vsix/UI/SafetyPaneControl.xaml"));
        XNamespace p = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        var body = Assert.Single(document.Descendants(p + "DataTemplate"), e => (string?)e.Attribute(x + "Key") == "ProcedureBodyTemplate");
        Assert.DoesNotContain("{Binding ContextLabel}", body.ToString());
        Assert.DoesNotContain("Based on this window's text", body.ToString());
        Assert.Contains("Body analysis incomplete", body.ToString());
        Assert.Single(body.Descendants(p + "Expander"));
        Assert.Empty(body.Descendants(p + "ToggleButton"));
        Assert.Contains(body.Descendants(p + "ItemsControl"), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding VisibleRows}" &&
            element.Attribute("ItemTemplate") == null);
        var finding = Assert.Single(document.Descendants(p + "DataTemplate"), e => (string?)e.Attribute(x + "Key") == "BodyFindingTemplate");
        var row = Assert.Single(finding.Elements(p + "Expander"));
        Assert.Equal("{StaticResource RowExpander}", (string?)row.Attribute("Style"));
        Assert.Equal("{StaticResource FindingHeaderTemplate}", (string?)row.Attribute("HeaderTemplate"));
        Assert.Single(finding.Descendants(p + "TextBlock")); // One short reason, without repeating SQL.
        var operationHeader = Assert.Single(document.Descendants(p + "DataTemplate"), e => (string?)e.Attribute(x + "Key") == "FindingHeaderTemplate");
        Assert.Contains("{Binding Body.Badge}", operationHeader.ToString());
        var badge = Assert.Single(document.Descendants(p + "DataTemplate"), e => (string?)e.Attribute(x + "Key") == "BodyBadgeTemplate");
        Assert.DoesNotContain("{Binding Label}", badge.ToString());
        Assert.DoesNotContain("{Binding Context}", badge.ToString());
        Assert.Equal(2, badge.Descendants(p + "TextBlock").Count()); // One scoped total for each kind of preview.
        Assert.Contains("{Binding Tooltip}", badge.ToString());
        foreach (var count in new[] { "InsideTotal", "RemoteTotal" })
            Assert.Contains("{Binding " + count + "}", badge.ToString());
        var preview = Assert.Single(document.Descendants(p + "DataTemplate"), e => (string?)e.Attribute(x + "Key") == "PreviewHeaderTemplate");
        Assert.Contains("{Binding Marker}", preview.ToString());
        foreach (var risk in new[] { "Writes", "Unknown", "Safe" }) Assert.Contains("{Binding DisplayCounts." + risk + "}", preview.ToString());
        Assert.DoesNotContain("RiskLabel", operationHeader.ToString());
        Assert.Contains(finding.Descendants(p + "Button"), e => (string?)e.Attribute("Click") == "OnSqlSourceClicked");
        Assert.Contains(finding.Descendants(p + "Button"), e => (string?)e.Attribute("Click") == "OnSqlExecutionClicked");
        Assert.DoesNotContain("executes now", document.ToString());

        Assert.Contains(finding.Descendants(p + "ContentControl"), element =>
            (string?)element.Attribute("Content") == "{Binding Body}" &&
            (string?)element.Attribute("ContentTemplate") == "{DynamicResource ProcedureBodyTemplate}");
        var group = Assert.Single(document.Descendants(p + "DataTemplate"), e => (string?)e.Attribute(x + "Key") == "GroupTemplate");
        Assert.DoesNotContain(group.Descendants(p + "ItemsControl"), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding Bodies}");
    }

    [Fact]
    public void PaneRowsAndSettingsUseShellThemeResources()
    {
        var repository = TestRepository.Root;
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/SafetyPaneControl.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace markup = "http://schemas.microsoft.com/winfx/2006/xaml";
        foreach (var key in new[] { "PaneItem", "RowExpander", "PageButton" })
        {
            var style = Assert.Single(document.Descendants(presentation + "Style"), element =>
                (string?)element.Attribute(markup + "Key") == key);
            Assert.Contains(style.Elements(presentation + "Setter"), element =>
                (string?)element.Attribute("Property") == "Foreground" &&
                ((string?)element.Attribute("Value"))?.Contains("EnvironmentColors.ToolWindowTextBrushKey") == true);
        }
        var settings = Assert.Single(document.Descendants(presentation + "Button"), element =>
            (string?)element.Attribute("Click") == "OnEditOptions");
        Assert.Equal("Settings...", (string?)settings.Attribute("Content"));
        Assert.Contains("ThemedDialogButtonStyleKey", (string?)settings.Attribute("Style"));
        Assert.DoesNotContain(document.Descendants().Attributes(), attribute =>
            attribute.Name.LocalName == "Value" && attribute.Value.StartsWith("#"));
    }

    [Theory]
    [InlineData("SettingsListDialog.xaml")]
    [InlineData("SettingsImportDialog.xaml")]
    public void SettingsDialogsIndicateSafeDefaultWithThemeAccent(string file)
    {
        var repository = TestRepository.Root;
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/", file));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        var cancel = Assert.Single(document.Descendants(presentation + "Button"), element =>
            (string?)element.Attribute("IsCancel") == "True");
        Assert.Equal("True", (string?)cancel.Attribute("IsDefault"));
        Assert.Contains("EnvironmentColors.SystemHighlightBrushKey", (string?)cancel.Attribute("BorderBrush"));
    }
}
