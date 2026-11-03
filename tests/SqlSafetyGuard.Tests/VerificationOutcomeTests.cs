using System.Diagnostics;
using System.Text.Json;

namespace SqlSafetyGuard.Tests;

public class VerificationOutcomeTests
{
    [Theory]
    [InlineData("@()", "@()", 2)]
    [InlineData("@(@{Scenario='a';Status='PASS'})", "@()", 0)]
    [InlineData("@(@{Scenario='a';Status='FAIL'})", "@('a')", 1)]
    [InlineData("@(@{Scenario='a';Status='BLOCKED'})", "@()", 2)]
    [InlineData("@(@{Scenario='a';Status='BLOCKED'})", "@('a')", 0)]
    [InlineData("@(@{Scenario='a';Status='PASS'})", "@('missing')", 2)]
    [InlineData("@(@{Scenario='a';Status='PASS'},@{Scenario='a';Status='PASS'})", "@()", 1)]
    public async Task ResultGateDistinguishesFailureAndIncompleteEvidence(string rows, string expected, int code)
    {
        if (!OperatingSystem.IsWindows()) return;
        var start = new ProcessStartInfo("powershell.exe")
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true,
        };
        var script = Path.Combine(TestRepository.Root, "tests/SqlSafetyGuard.ExtensionChecks/CheckOutcome.ps1").Replace("'", "''");
        foreach (var argument in new[] { "-NoProfile", "-Command", $". '{script}'; Get-CheckOutcome -Results {rows} -ExpectedBlockedScenarios {expected} | ConvertTo-Json -Compress" })
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var errors = process.StandardError.ReadToEndAsync();
        try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30)); }
        catch (TimeoutException) { process.Kill(entireProcessTree: true); throw; }
        Assert.True(process.ExitCode == 0, await errors);
        using var result = JsonDocument.Parse(await output);
        Assert.Equal(code, result.RootElement.GetProperty("ExitCode").GetInt32());
    }
}
