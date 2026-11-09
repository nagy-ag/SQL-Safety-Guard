using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace SqlSafetyGuard.Tests;

/// <summary>Keeps docs/README.md the single, complete list of current documents (history/ holds dated snapshots).</summary>
public class DocsIndexTests
{
    private static readonly string Docs = Path.Combine(TestRepository.Root, "docs");

    private static readonly Regex Link = new(@"\]\(([^)#\s]+)(#[^)]*)?\)");

    /// <summary>Current documents live in an audience folder; only the map and the status page sit directly in docs/.</summary>
    private static readonly string[] CurrentFolders = { "user/", "it/", "dev/", "safety/", "verification/rules/" };

    private static IEnumerable<string> CurrentDocuments() => Directory.EnumerateFiles(Docs, "*.md", SearchOption.AllDirectories)
        .Select(path => Path.GetRelativePath(Docs, path).Replace('\\', '/'))
        .Where(path => !path.StartsWith("history/", StringComparison.Ordinal) && !IsMovedPage(path));

    /// <summary>A page kept at an old path only so that frozen snapshots' links still resolve.</summary>
    private static bool IsMovedPage(string path) =>
        File.ReadLines(Path.Combine(Docs, path)).FirstOrDefault()?.StartsWith("# Moved:", StringComparison.Ordinal) == true;

    [Fact]
    public void EveryCurrentDocumentIsListedOnTheStartPage()
    {
        var index = File.ReadAllText(Path.Combine(Docs, "README.md"));
        var unlisted = CurrentDocuments()
            .Where(path => path != "README.md")
            .Where(path => !index.Contains("](" + path + ")", StringComparison.Ordinal))
            .ToList();
        Assert.True(unlisted.Count == 0, "Add to docs/README.md or move to docs/history/: " + string.Join(", ", unlisted));
    }

    [Fact]
    public void CurrentDocumentsLiveInTheirAudienceFolder()
    {
        var misplaced = CurrentDocuments()
            .Where(path => path != "README.md" && path != "status.md")
            .Where(path => !CurrentFolders.Any(folder => path.StartsWith(folder, StringComparison.Ordinal)))
            .ToList();
        Assert.True(misplaced.Count == 0, "Move into user/, it/, dev/ or safety/ (or history/ if dated): " + string.Join(", ", misplaced));
    }

    [Fact]
    public void MovedPagesPointToACurrentDocument()
    {
        var moved = Directory.EnumerateFiles(Docs, "*.md", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(Docs, path).Replace('\\', '/'))
            .Where(path => !path.StartsWith("history/", StringComparison.Ordinal) && IsMovedPage(path))
            .ToList();
        Assert.NotEmpty(moved);
        foreach (var path in moved)
        {
            var text = File.ReadAllText(Path.Combine(Docs, path));
            Assert.True(text.Length < 600, "A moved page should only point to the new location: " + path);
            var targets = Link.Matches(text).Select(match => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.Combine(Docs, path))!, match.Groups[1].Value)));
            Assert.True(targets.Any(target => target.EndsWith(".md", StringComparison.Ordinal) && File.Exists(target) &&
                !IsMovedPage(Path.GetRelativePath(Docs, target))), "A moved page must link to a current document: " + path);
        }
    }

    [Fact]
    public void RelativeLinksInDocsResolve()
    {
        var broken = new List<string>();
        var maintainedRoots = new[] { "docs", ".agents", ".claude", "src", "tests", "tools", "scripts" };
        var documents = maintainedRoots.SelectMany(directory => Directory.EnumerateFiles(
                Path.Combine(TestRepository.Root, directory), "*.md", SearchOption.AllDirectories))
            .Concat(Directory.EnumerateFiles(TestRepository.Root, "*.md", SearchOption.TopDirectoryOnly));
        foreach (var path in documents
                     .Where(path => !Path.GetRelativePath(TestRepository.Root, path).Split(Path.DirectorySeparatorChar)
                         .Any(part => part is "bin" or "obj" or ".git" or ".vs")))
        {
            foreach (Match match in Link.Matches(File.ReadAllText(path)))
            {
                var target = match.Groups[1].Value;
                if (target.Contains('<')) continue; // Instruction placeholders, such as <relative>/README.md.
                if (Regex.IsMatch(target, "^(https?:|mailto:|0x)")) continue;
                var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, target));
                if (!File.Exists(resolved) && !Directory.Exists(resolved))
                    broken.Add(Path.GetRelativePath(Docs, path).Replace('\\', '/') + " -> " + target);
            }
        }
        Assert.True(broken.Count == 0, "Broken links: " + string.Join(", ", broken));
    }

    [Fact]
    public void HistorySnapshotsPointBackToTheCurrentState()
    {
        // Original evidence protected by a checksum manifest is preserved byte-for-byte; its archive README provides context.
        var protectedFiles = ProtectedFiles().Select(entry => entry.Path).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = Directory.EnumerateFiles(Path.Combine(Docs, "history"), "*.md", SearchOption.AllDirectories)
            .Where(path => !protectedFiles.Contains(path) && Path.GetFileName(path) != "README.md")
            .Where(path => !File.ReadLines(path).First().StartsWith("> **Snapshot", StringComparison.Ordinal))
            .Select(path => Path.GetRelativePath(Docs, path).Replace('\\', '/'))
            .ToList();
        Assert.True(missing.Count == 0, "History files need the snapshot header: " + string.Join(", ", missing));
    }

    [Fact]
    public void PreservedHistoricalEvidenceMatchesItsChecksums()
    {
        foreach (var entry in ProtectedFiles())
        {
            // Historical ZIP bundles are deliberately untracked and may be absent in a fresh checkout.
            if (!File.Exists(entry.Path) && Path.GetExtension(entry.Path) == ".zip") continue;
            Assert.True(File.Exists(entry.Path), "Missing historical evidence: " + entry.Path);
            Assert.Equal(entry.Hash, Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(entry.Path))).ToLowerInvariant());
        }
    }

    private static IEnumerable<(string Path, string Hash)> ProtectedFiles()
    {
        foreach (var manifest in Directory.EnumerateFiles(Path.Combine(Docs, "history"), "SHA256SUMS.txt", SearchOption.AllDirectories))
        foreach (var line in File.ReadLines(manifest).Where(line => !string.IsNullOrWhiteSpace(line)))
        {
            var match = Regex.Match(line, @"^([a-fA-F0-9]{64})\s+\*?(.+)$");
            Assert.True(match.Success, "Invalid checksum entry: " + manifest);
            var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(manifest)!, match.Groups[2].Value));
            Assert.StartsWith(Path.GetDirectoryName(manifest)! + Path.DirectorySeparatorChar, path, StringComparison.OrdinalIgnoreCase);
            yield return (path, match.Groups[1].Value.ToLowerInvariant());
        }
    }
}
