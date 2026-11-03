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
        foreach (string name in new[] { "CollapsedGlyph", "CollapsedRedCount", "CollapsedYellowCount" })
            Assert.Single(strip.Descendants(presentation + "TextBlock"), element =>
                (string?)element.Attribute(markup + "Name") == name);
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
