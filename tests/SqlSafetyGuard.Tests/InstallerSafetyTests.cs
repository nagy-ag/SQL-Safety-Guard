using System.Diagnostics;

namespace SqlSafetyGuard.Tests;

public class InstallerSafetyTests
{
    [Fact]
    public async Task WindowsInstallerRejectsUnsafeArchivesAndPreservesPreviousInstall()
    {
        if (!OperatingSystem.IsWindows()) return;
        var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        var fixtures = Path.Combine(AppContext.BaseDirectory, "installer-fixtures-" + Guid.NewGuid().ToString("N"));
        var start = new ProcessStartInfo("powershell.exe")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        foreach (var argument in new[] { "-NoProfile", "-ExecutionPolicy", "Bypass", "-File",
            Path.Combine(repository, "scripts/InstallerSafetyTests.ps1"), "-FixtureRoot", fixtures })
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var errors = process.StandardError.ReadToEndAsync();
        try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(90)); }
        catch (TimeoutException)
        {
            process.Kill(entireProcessTree: true);
            Assert.Fail("Installer fixtures exceeded 90 seconds.");
        }
        Assert.True(process.ExitCode == 0, await output + await errors);
    }
}
