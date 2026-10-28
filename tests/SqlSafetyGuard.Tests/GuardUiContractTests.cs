using System.Xml.Linq;

namespace SqlSafetyGuard.Tests;

public class GuardUiContractTests
{
    [Fact]
    public void CollapsedPaneHasPassiveBadgesAndOneExpandTarget()
    {
        var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/SafetyPaneControl.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace markup = "http://schemas.microsoft.com/winfx/2006/xaml";
        var strip = Assert.Single(document.Descendants(presentation + "Border"),
            element => (string?)element.Attribute(markup + "Name") == "CollapsedStrip");

        Assert.Equal("Transparent", (string?)strip.Attribute("Background"));
        Assert.Equal("OnToggleClicked", (string?)strip.Attribute("MouseLeftButtonUp"));
        Assert.Empty(strip.Descendants(presentation + "Button"));
        Assert.Empty(strip.Descendants(presentation + "RotateTransform"));
        Assert.DoesNotContain(strip.Descendants(presentation + "TextBlock"),
            element => element.Attribute("Text") != null);
        Assert.DoesNotContain(strip.Descendants().Attributes(),
            attribute => attribute.Name.LocalName.StartsWith("Mouse", StringComparison.Ordinal));
        var badges = strip.Descendants(presentation + "Grid").ToList();
        Assert.Equal(new[] { "CollapsedStatus", "CollapsedRed", "CollapsedYellow" },
            badges.Select(element => (string?)element.Attribute(markup + "Name")));
        Assert.All(badges, badge =>
        {
            Assert.Equal("20", (string?)badge.Attribute("Width"));
            Assert.Equal("20", (string?)badge.Attribute("Height"));
            Assert.Single(badge.Elements(presentation + "Ellipse"));
        });
        Assert.All(strip.Descendants(presentation + "Viewbox"),
            viewbox => Assert.Equal("DownOnly", (string?)viewbox.Attribute("StretchDirection")));
    }

    [Fact]
    public void ConfirmationButtonsHaveSafeKeyboardDefaults()
    {
        var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        var document = XDocument.Load(Path.Combine(repository, "src/SqlSafetyGuard.Vsix/UI/ConfirmExecutionDialog.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace markup = "http://schemas.microsoft.com/winfx/2006/xaml";
        var buttons = document.Descendants(presentation + "Button").ToList();
        var run = Assert.Single(buttons, button => (string?)button.Attribute(markup + "Name") == "RunButton");
        var cancel = Assert.Single(buttons, button => (string?)button.Attribute(markup + "Name") == "CancelButton");
        Assert.Equal("Run anyway", (string?)run.Attribute("Content"));
        Assert.NotEqual("True", (string?)run.Attribute("IsDefault"));
        Assert.Empty(run.Descendants(presentation + "AccessText"));
        Assert.Equal("True", (string?)cancel.Attribute("IsDefault"));
        Assert.Equal("True", (string?)cancel.Attribute("IsCancel"));
    }
}
