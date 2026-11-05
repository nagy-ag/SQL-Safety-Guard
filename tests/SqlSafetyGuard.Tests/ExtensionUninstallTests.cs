using System.Diagnostics;
using System.Xml.Linq;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.Tests;

public class ExtensionUninstallTests
{
    private static readonly string LocalData = Path.Combine(Path.GetTempPath(), "ssg-uninstall-test", "Local");
    private static readonly string Host = Path.Combine(Path.GetTempPath(), "ssg-uninstall-test", "SSMS IDE", "SSMS.exe");
    private const string Ini = "[Other]\nInstallationID=ignored\n[Info]\nInstallationID=instance22\nSKU=SSMS";

    private static string Assembly(string suffix = "", string id = "instance22") =>
        Path.Combine(LocalData, "Microsoft", "SSMS", "22.0_" + id + suffix, "Extensions", "random-id", "SqlSafetyGuard.dll");

    private static ExtensionUninstallPlan Plan(string suffix = "", params string[] arguments) =>
        ExtensionUninstallPlan.Create(Host, Ini, Assembly(suffix), LocalData, new[] { Host }.Concat(arguments).ToArray());

    [Fact]
    public void DefaultProfileLaunchesOnlyTheAdjacentInstallerForTheCurrentInstanceAndExtension()
    {
        var plan = Plan();
        ProcessStartInfo? launched = null;
        Assert.True(plan.Request(() => true, start => launched = start));
        Assert.NotNull(launched);
        Assert.Equal(Path.Combine(Path.GetDirectoryName(Host)!, "VSIXInstaller.exe"), launched.FileName);
        Assert.Equal(Path.GetDirectoryName(Host), launched.WorkingDirectory);
        Assert.Equal("/instanceIds:instance22 /uninstall:" + ExtensionUninstallPlan.ExtensionId, launched.Arguments);
        Assert.False(launched.UseShellExecute);
        var manifest = XDocument.Load(Path.Combine(TestRepository.Root, "src/SqlSafetyGuard.Vsix/source.extension.vsixmanifest"));
        Assert.Equal(ExtensionUninstallPlan.ExtensionId,
            (string?)manifest.Descendants().Single(e => e.Name.LocalName == "Identity").Attribute("Id"));
    }

    [Theory]
    [InlineData("/rootSuffix", "Exp")]
    [InlineData("/ROOTSUFFIX", "Exp")]
    [InlineData("-rootSuffix", "Exp")]
    [InlineData("/rootSuffix:Exp", null)]
    [InlineData("-rootSuffix:Exp", null)]
    public void ExperimentalProfileNeverTargetsTheNormalProfile(string option, string? value)
    {
        var plan = Plan("Exp", value == null ? new[] { option } : new[] { option, value });
        ProcessStartInfo? launched = null;
        plan.Request(() => true, start => launched = start);
        Assert.Equal("Exp", plan.RootSuffix);
        Assert.Equal("/instanceIds:instance22 /rootSuffix:Exp /uninstall:" + ExtensionUninstallPlan.ExtensionId, launched!.Arguments);
    }

    [Fact]
    public void CancelNeverStartsAnInstaller()
    {
        int confirmations = 0;
        Assert.False(Plan().Request(() => { confirmations++; return false; }, _ => Assert.Fail("Cancellation must not start an installer.")));
        Assert.Equal(1, confirmations);
    }

    [Fact]
    public void FailedConfirmationOrLaunchDoesNotReportSuccessfulHandoff()
    {
        Assert.Throws<InvalidOperationException>(() => Plan().Request(
            () => throw new InvalidOperationException("confirmation failed"), _ => Assert.Fail("Must not launch.")));
        Assert.Throws<System.ComponentModel.Win32Exception>(() => Plan().Request(
            () => true, _ => throw new System.ComponentModel.Win32Exception("launch failed")));
    }

    [Theory]
    [InlineData("/rootSuffix")]
    [InlineData("/rootSuffix:")]
    [InlineData("/rootSuffix=Exp")]
    [InlineData("/rootSuffix:Exp /quiet")]
    [InlineData("/rootSuffix:../Exp")]
    [InlineData("/rootSuffix:Exp\" /quiet")]
    [InlineData("/rootSuffix:Exp\n")]
    public void InvalidProfileCannotBecomeInstallerArguments(string argument) =>
        Assert.Throws<InvalidOperationException>(() => Plan("", argument));

    [Fact]
    public void RepeatedProfileOptionsAreRejectedEvenWhenTheyAgree() =>
        Assert.Throws<InvalidOperationException>(() => Plan("Exp", "/rootSuffix", "Exp", "/rootSuffix:Exp"));

    [Theory]
    [InlineData("[Info]\nSKU=SSMS")]
    [InlineData("[Other]\nInstallationID=instance22")]
    [InlineData("[Info]\nInstallationID=")]
    [InlineData("[Info]\nInstallationID=instance22\nInstallationID=instance22")]
    [InlineData("[Info]\nInstallationID=instance22 /quiet")]
    [InlineData("[Info]\nInstallationID=../instance22")]
    [InlineData("[Info]\nInstallationID=instance22\" /admin")]
    public void MissingAmbiguousOrUnsafeInstallationIdIsRejected(string ini) =>
        Assert.Throws<InvalidOperationException>(() => ExtensionUninstallPlan.Create(Host, ini, Assembly(), LocalData, new[] { Host }));

    [Theory]
    [InlineData("", "Exp", "instance22")]
    [InlineData("Exp", "", "instance22")]
    [InlineData("", "", "other-instance")]
    public void LoadedAssemblyMustAgreeWithTheRunningProfileAndInstance(string runningSuffix, string loadedSuffix, string loadedId)
    {
        var arguments = runningSuffix.Length == 0 ? new[] { Host } : new[] { Host, "/rootSuffix", runningSuffix };
        Assert.Throws<InvalidOperationException>(() => ExtensionUninstallPlan.Create(
            Host, Ini, Assembly(loadedSuffix, loadedId), LocalData, arguments));
    }

    [Fact]
    public void ManualCopyAndUnrecognizedPathsRequireAdministratorInstructions()
    {
        var root = Path.Combine(LocalData, "Microsoft", "SSMS", "22.0_instance22", "Extensions");
        foreach (var path in new[]
        {
            Path.Combine(Path.GetDirectoryName(Host)!, "Extensions", "SqlSafetyGuard", "SqlSafetyGuard.dll"),
            Path.Combine(root + "Other", "random-id", "SqlSafetyGuard.dll"),
            Path.Combine(root, "SqlSafetyGuard.dll"),
            Path.Combine(root, "random-id", "..", "..", "elsewhere", "SqlSafetyGuard.dll")
        })
        {
            var error = Assert.Throws<InvalidOperationException>(() =>
                ExtensionUninstallPlan.Create(Host, Ini, path, LocalData, new[] { Host }));
            Assert.Contains("administrator", error.Message);
            Assert.Contains("uninstall.ps1", error.Message);
        }
    }

    [Fact]
    public void NonSsmsHostIsRejected() =>
        Assert.Throws<InvalidOperationException>(() => ExtensionUninstallPlan.Create(
            Path.Combine(Path.GetDirectoryName(Host)!, "devenv.exe"), Ini, Assembly(), LocalData, new[] { Host }));

    [Fact]
    public void ToolsSubmenuHasAParentGroupSoSsmsCanDisplayItsCommands()
    {
        XNamespace ns = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable";
        var commands = XDocument.Load(Path.Combine(TestRepository.Root, "src/SqlSafetyGuard.Vsix/SqlSafetyGuardPackage.vsct"));
        var menu = commands.Descendants(ns + "Menu").Single(e => (string?)e.Attribute("id") == "SqlSafetyGuardMenu");
        var parent = menu.Element(ns + "Parent")!;
        // SSMS silently omits a submenu parented directly to a menu instead of a group.
        var group = Assert.Single(commands.Descendants(ns + "Group"), e =>
            (string?)e.Attribute("guid") == (string?)parent.Attribute("guid") &&
            (string?)e.Attribute("id") == (string?)parent.Attribute("id"));
        Assert.Equal("guidSHLMainMenu", (string?)group.Element(ns + "Parent")?.Attribute("guid"));
        Assert.Equal("IDM_VS_MENU_TOOLS", (string?)group.Element(ns + "Parent")?.Attribute("id"));
    }

    [Fact]
    public void MenuCommandIsRegisteredWithoutToolbarPlacementOrShortcutAndDefaultsToNo()
    {
        XNamespace ns = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable";
        var commands = XDocument.Load(Path.Combine(TestRepository.Root, "src/SqlSafetyGuard.Vsix/SqlSafetyGuardPackage.vsct"));
        var button = Assert.Single(commands.Descendants(ns + "Button"), e => (string?)e.Attribute("id") == "cmdidUninstall");
        string groupId = (string)button.Element(ns + "Parent")!.Attribute("id")!;
        var group = commands.Descendants(ns + "Group").Single(e => (string?)e.Attribute("id") == groupId);
        Assert.Equal("SqlSafetyGuardMenu", (string?)group.Element(ns + "Parent")?.Attribute("id"));
        Assert.DoesNotContain(commands.Descendants(ns + "KeyBinding"), e => (string?)e.Attribute("id") == "cmdidUninstall");
        Assert.DoesNotContain(commands.Descendants(ns + "CommandPlacement"), e => (string?)e.Attribute("id") == "cmdidUninstall");
        string adapter = File.ReadAllText(Path.Combine(TestRepository.Root, "src/SqlSafetyGuard.Vsix/Commands/UninstallCommand.cs"));
        Assert.Contains("OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND) == (int)VSConstants.MessageBoxResult.IDYES", adapter);
        Assert.Contains("UninstallCommand.Initialize(this, commandService)",
            File.ReadAllText(Path.Combine(TestRepository.Root, "src/SqlSafetyGuard.Vsix/SqlSafetyGuardPackage.cs")));
    }
}
