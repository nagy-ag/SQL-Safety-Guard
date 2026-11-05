using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

namespace SqlSafetyGuard.Tests;

// Starting multiple Windows PowerShell processes competes with the analyzer's
// wall-clock resource checks. Run these external-process fixtures separately.
[CollectionDefinition("Signing kit process fixtures", DisableParallelization = true)]
public class SigningKitProcessCollection { }

[Collection("Signing kit process fixtures")]
public class SigningKitTests
{
    [Theory]
    [InlineData("valid", true, "Verified unsigned input")]
    [InlineData("wrong-hash", false, "checksum mismatch")]
    [InlineData("duplicate-hash", false, "exactly one entry")]
    [InlineData("signed", false, "already has package signatures")]
    [InlineData("opc-signed", false, "already has package signatures")]
    [InlineData("wrong-version", false, "identity/version")]
    [InlineData("existing-output", false, "already exists")]
    public async Task SigningKitChecksInputsBeforeAccessingKeysOrTools(string scenario, bool success, string message)
    {
        if (!OperatingSystem.IsWindows()) return;
        var folder = Path.Combine(Path.GetTempPath(), "sg-signing-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            var input = Path.Combine(folder, "SqlSafetyGuard-1.0.1.vsix");
            using (var zip = ZipFile.Open(input, ZipArchiveMode.Create))
            {
                using (var writer = new StreamWriter(zip.CreateEntry("extension.vsixmanifest").Open()))
                    writer.Write("<PackageManifest><Metadata><Identity Id=\"SqlSafetyGuard.fd93cdc5-5bc3-4b7b-afaa-e0c208810558\" Version=\"" +
                        (scenario == "wrong-version" ? "1.0.0" : "1.0.1") + "\" /></Metadata></PackageManifest>");
                if (scenario == "signed") zip.CreateEntry("_xmlsignatures/sig1.psdsxs");
                if (scenario == "opc-signed") zip.CreateEntry("package/services/digital-signature/xml-signature/sig1.psdsxs");
            }
            var original = File.ReadAllBytes(input);
            var checksum = Convert.ToHexString(SHA256.HashData(original));
            var sums = Path.Combine(folder, "SHA256SUMS.txt");
            var line = (scenario == "wrong-hash" ? new string('0', 64) : checksum) + "  SqlSafetyGuard-1.0.1.vsix\n";
            File.WriteAllText(sums, scenario == "duplicate-hash" ? line + line : line);
            var output = Path.Combine(folder, "company.vsix");
            if (scenario == "existing-output") File.WriteAllText(output, "preserve");
            var start = new ProcessStartInfo("powershell.exe")
            {
                UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true
            };
            foreach (var argument in new[] { "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", Path.Combine(TestRepository.Root, "signing/Sign-CompanyVsix.ps1"),
                "-VsixPath", input, "-ChecksumsPath", sums, "-SignToolPath", "NONEXISTENT-SIGN-TOOL" }) start.ArgumentList.Add(argument);
            if (scenario == "existing-output")
            {
                foreach (var argument in new[] { "-OutputPath", output, "-PublicCertificatePath", "NONEXISTENT.cer" }) start.ArgumentList.Add(argument);
            }
            else start.ArgumentList.Add("-CheckOnly");
            using var process = Process.Start(start)!;
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(45)); }
            catch (TimeoutException) { process.Kill(entireProcessTree: true); throw; }
            var log = await stdout + await stderr;
            Assert.True(success == (process.ExitCode == 0), log);
            Assert.Contains(message, log);
            Assert.Equal(original, File.ReadAllBytes(input));
            if (scenario == "existing-output") Assert.Equal("preserve", File.ReadAllText(output));
            else Assert.False(File.Exists(output));
        }
        finally { Directory.Delete(folder, recursive: true); }
    }
}
