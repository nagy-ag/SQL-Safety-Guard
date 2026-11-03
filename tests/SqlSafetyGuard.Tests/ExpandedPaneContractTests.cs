using System.Xml.Linq;

namespace SqlSafetyGuard.Tests;

public class ExpandedPaneContractTests
{
    private static readonly XNamespace Presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private static readonly XNamespace Markup = "http://schemas.microsoft.com/winfx/2006/xaml";
    private static XDocument ReadPane() => XDocument.Load(Path.Combine(
        TestRepository.Root,
        "src/SqlSafetyGuard.Vsix/UI/SafetyPaneControl.xaml"));
    private static XElement Named(XContainer parent, string name) => Assert.Single(parent.Descendants(),
        element => (string?)element.Attribute(Markup + "Name") == name);

    [Fact]
    public void HeaderSharesScopeEnvironmentAndNavigationAcrossPages()
    {
        var document = ReadPane();
        var expanded = Named(document, "Expanded");
        // Locate the shared header by its content, independently of pixel dimensions.
        var header = Assert.Single(expanded.Elements(Presentation + "Border"), element =>
            element.Descendants().Any(child => (string?)child.Attribute(Markup + "Name") == "ScopeText"));
        Assert.Equal("Whole window", (string?)Named(header, "ScopeText").Attribute("Text"));
        Assert.Equal("Collapsed", (string?)Named(header, "EnvironmentTag").Attribute("Visibility"));
        Assert.Equal("Analysis", (string?)Named(header, "AnalysisButton").Attribute("ToolTip"));
        Assert.Equal("Settings", (string?)Named(header, "SettingsButton").Attribute("ToolTip"));
        Assert.Equal("OnAnalysisPage", (string?)Named(header, "AnalysisButton").Attribute("Checked"));
        Assert.Equal("OnSettingsPage", (string?)Named(header, "SettingsButton").Attribute("Checked"));
        Assert.Equal("OnCollapseClicked", (string?)Named(header, "CollapseButton").Attribute("Click"));
        foreach (string name in new[] { "HeaderGlyph", "HeaderRedCount", "HeaderYellowCount" })
            Assert.Equal(Presentation + "TextBlock", Named(header, name).Name);
        Assert.DoesNotContain(header.Descendants().Attributes(), attribute =>
            new[] { "Safe", "Not safe", "Needs review", "Server:" }.Contains(attribute.Value));
        Assert.DoesNotContain(header, Named(document, "AnalysisPage").Ancestors());
        Assert.DoesNotContain(header, Named(document, "SettingsPage").Ancestors());
    }

    [Fact]
    public void ObjectRowsUseOneSharedTemplateAndExplicitExpandableDetails()
    {
        var document = ReadPane();
        Assert.Equal("Bottom", (string?)Named(document, "SafeSection").Attribute("DockPanel.Dock"));
        foreach (string name in new[] { "GroupList", "SafeList" })
        {
            var list = Named(document, name);
            Assert.Equal("{StaticResource ObjectList}", (string?)list.Attribute("Style"));
            Assert.Equal("OnGroupSelectionChanged", (string?)list.Attribute("SelectionChanged"));
            Assert.Equal("OnGroupDoubleClick", (string?)list.Attribute("MouseDoubleClick"));
            Assert.Equal("OnGroupKeyDown", (string?)list.Attribute("KeyDown"));
        }
        Assert.Empty(document.Descendants(Presentation + "GridViewColumn"));
        var template = Assert.Single(document.Descendants(Presentation + "DataTemplate"),
            element => (string?)element.Attribute(Markup + "Key") == "GroupTemplate");
        Assert.DoesNotContain(template.Descendants(Presentation + "TextBlock"), element =>
            (string?)element.Attribute("Text") == "{Binding DatabasePrefix}");
        Assert.Contains(template.Descendants(Presentation + "TextBlock"), element =>
            (string?)element.Attribute("Text") == "{Binding ObjectName}" &&
            (string?)element.Attribute("ToolTip") == "{Binding DisplayName}");
        var expander = Assert.Single(template.Elements(Presentation + "Expander"));
        Assert.Equal("{Binding IsExpanded, Mode=TwoWay}", (string?)expander.Attribute("IsExpanded"));
        Assert.Equal("OnGroupExpanded", (string?)expander.Attribute("Expanded"));
        var header = Assert.Single(expander.Elements(Presentation + "Expander.Header"));
        foreach (string binding in new[] { "ObjectName", "Subtitle", "Line" })
            Assert.Contains(header.Descendants(Presentation + "TextBlock"), element =>
                (string?)element.Attribute("Text") == "{Binding " + binding + "}");
        Assert.DoesNotContain(template.Descendants(Presentation + "DataTrigger"), element =>
            ((string?)element.Attribute("Binding"))?.Contains("IsSelected") == true);
        foreach (string binding in new[] { "Reason", "OperationSummary", "LineSummary" })
            Assert.Contains(expander.Descendants(Presentation + "TextBlock"), element =>
                (string?)element.Attribute("Text") == "{Binding " + binding + "}" &&
                (string?)element.Attribute("TextWrapping") == "Wrap");
        var style = Assert.Single(document.Descendants(Presentation + "Style"),
            element => (string?)element.Attribute(Markup + "Key") == "RowExpander");
        Assert.Equal("{Binding IsExpanded, RelativeSource={RelativeSource TemplatedParent}, Mode=TwoWay}",
            (string?)Named(style, "RowHeader").Attribute("IsChecked"));
        Assert.Equal("Collapsed", (string?)Named(style, "RowDetails").Attribute("Visibility"));
        Assert.Contains(style.Descendants(Presentation + "Trigger"), element =>
            (string?)element.Attribute("Property") == "IsExpanded" && (string?)element.Attribute("Value") == "True");
    }

    [Fact]
    public void AnalysisBackgroundCanClearSelectionWithoutConsumingRowClicks()
    {
        var document = ReadPane();
        var analysis = Named(document, "AnalysisPage");
        Assert.Equal("Transparent", (string?)analysis.Attribute("Background"));
        Assert.Equal("OnAnalysisBackgroundMouseDown", (string?)analysis.Attribute("PreviewMouseLeftButtonDown"));
        Assert.Null(Named(document, "SettingsPage").Attribute("PreviewMouseLeftButtonDown"));
    }

    [Fact]
    public void SettingsAreReadOnlyWithExplicitOptionsActionAndPatternProvenance()
    {
        var document = ReadPane();
        var settings = Named(document, "SettingsPage");
        Assert.Empty(settings.Descendants(Presentation + "TextBox"));
        Assert.Empty(settings.Descendants(Presentation + "CheckBox"));
        var button = Assert.Single(settings.Descendants(Presentation + "Button"));
        Assert.Equal("Settings...", (string?)button.Attribute("Content"));
        Assert.Equal("OnEditOptions", (string?)button.Attribute("Click"));
        var template = Assert.Single(document.Descendants(Presentation + "DataTemplate"),
            element => (string?)element.Attribute(Markup + "Key") == "PatternTemplate");
        Assert.Contains(template.Descendants(Presentation + "TextBlock"), element =>
            (string?)element.Attribute("Text") == "{Binding Pattern}");
        Assert.Contains(template.Descendants(Presentation + "TextBlock"), element =>
            (string?)element.Attribute("Text") == "{Binding Source}");
    }
}
