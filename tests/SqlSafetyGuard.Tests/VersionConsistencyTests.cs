using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Tests;

/// <summary>One release number everywhere: the VSIX manifest, both assemblies and the newest changelog entry.</summary>
public class VersionConsistencyTests
{
    private static string Read(string path) => File.ReadAllText(Path.Combine(RuleInventoryTests.Root, path));

    private static string Match(string text, string pattern) =>
        Regex.Match(text, pattern) is { Success: true } match ? match.Groups[1].Value : throw new InvalidDataException(pattern);

    [Fact]
    public void ManifestAssembliesAndChangelogAgree()
    {
        var manifest = Match(Read("src/SqlSafetyGuard.Vsix/source.extension.vsixmanifest"), "<Identity [^>]*Version=\"([0-9.]+)\"");
        var vsix = Match(Read("src/SqlSafetyGuard.Vsix/Properties/AssemblyInfo.cs"), "AssemblyFileVersion\\(\"([0-9.]+)\"\\)");
        var analyzer = Match(Read("src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj"), "<FileVersion>([0-9.]+)</FileVersion>");
        var changelog = Match(Read("CHANGELOG.md"), "(?m)^## \\[([0-9.]+)\\]");

        Assert.Matches("^[0-9]+\\.[0-9]+\\.[0-9]+$", manifest);
        Assert.Equal(manifest + ".0", vsix);
        Assert.Equal(manifest + ".0", analyzer);
        Assert.Equal(manifest, changelog);
    }
}
