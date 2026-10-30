using System.Text.RegularExpressions;

namespace SqlSafetyGuard.Tests;

/// <summary>Keeps docs/README.md the single, complete list of current documents (history/ holds dated snapshots).</summary>
public class DocsIndexTests
{
    private static readonly string Docs = Path.Combine(RuleInventoryTests.Root, "docs");

    private static readonly Regex Link = new(@"\]\(([^)#\s]+)(#[^)]*)?\)");

    [Fact]
    public void EveryCurrentDocumentIsListedOnTheStartPage()
    {
        var index = File.ReadAllText(Path.Combine(Docs, "README.md"));
        var unlisted = Directory.EnumerateFiles(Docs, "*.md", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(Docs, path).Replace('\\', '/'))
            .Where(path => path != "README.md" && !path.StartsWith("history/", StringComparison.Ordinal))
            .Where(path => !index.Contains("](" + path + ")", StringComparison.Ordinal))
            .ToList();
        Assert.True(unlisted.Count == 0, "Add to docs/README.md or move to docs/history/: " + string.Join(", ", unlisted));
    }

    [Fact]
    public void RelativeLinksInDocsResolve()
    {
        var broken = new List<string>();
        foreach (var path in Directory.EnumerateFiles(Docs, "*.md", SearchOption.AllDirectories))
        {
            foreach (Match match in Link.Matches(File.ReadAllText(path)))
            {
                var target = match.Groups[1].Value;
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
        // The 2026-10-27-all-kinds files are SHA-256 protected and cannot take a header; the history index covers them.
        var missing = Directory.EnumerateFiles(Path.Combine(Docs, "history"), "*.md", SearchOption.AllDirectories)
            .Where(path => !path.Replace('\\', '/').Contains("/2026-10-27-all-kinds/") && Path.GetFileName(path) != "README.md")
            .Where(path => !File.ReadLines(path).First().StartsWith("> **Snapshot", StringComparison.Ordinal))
            .Select(path => Path.GetRelativePath(Docs, path).Replace('\\', '/'))
            .ToList();
        Assert.True(missing.Count == 0, "History files need the snapshot header: " + string.Join(", ", missing));
    }
}
