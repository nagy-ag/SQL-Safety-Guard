using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Tests;

/// <summary>One release number in the VSIX manifest and both assemblies.</summary>
public class VersionConsistencyTests
{
    private static string Read(string path) => File.ReadAllText(Path.Combine(TestRepository.Root, path));

    private static string Match(string text, string pattern) =>
        Regex.Match(text, pattern) is { Success: true } match ? match.Groups[1].Value : throw new InvalidDataException(pattern);

    [Fact]
    public void ManifestAndAssemblyVersionsAgree()
    {
        var manifest = Match(Read("src/SqlSafetyGuard.Vsix/source.extension.vsixmanifest"), "<Identity [^>]*Version=\"([0-9.]+)\"");
        var vsix = Match(Read("src/SqlSafetyGuard.Vsix/Properties/AssemblyInfo.cs"), "AssemblyFileVersion\\(\"([0-9.]+)\"\\)");
        var analyzer = Match(Read("src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj"), "<FileVersion>([0-9.]+)</FileVersion>");

        Assert.Matches("^[0-9]+\\.[0-9]+\\.[0-9]+$", manifest);
        Assert.Equal(manifest + ".0", vsix);
        Assert.Equal(manifest + ".0", analyzer);
        Assert.Equal(manifest + ".0", Match(Read("src/SqlSafetyGuard.Vsix/Properties/AssemblyInfo.cs"), "AssemblyVersion\\(\"([0-9.]+)\"\\)"));
        Assert.Equal(manifest + ".0", Match(Read("src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj"), "<AssemblyVersion>([0-9.]+)</AssemblyVersion>"));
        Assert.Equal(manifest, Match(Read("src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj"), "<Version>([0-9.]+)</Version>"));
    }
}
