using System.Xml.Linq;

namespace SqlSafetyGuard.Tests;

public class GuardUiContractTests
{
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
