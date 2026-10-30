namespace SqlSafetyGuard.Tests;

/// <summary>
/// .agents/skills is the cross-agent original; Claude Code only reads .claude/skills, which must stay an identical copy.
/// </summary>
public class AgentSkillsSyncTests
{
    private static readonly string Original = Path.Combine(RuleInventoryTests.Root, ".agents", "skills");
    private static readonly string ClaudeCopy = Path.Combine(RuleInventoryTests.Root, ".claude", "skills");

    [Fact]
    public void ClaudeSkillsAreAnIdenticalCopyOfAgentSkills()
    {
        var original = Files(Original);
        var copy = Files(ClaudeCopy);
        Assert.NotEmpty(original);
        var problems = original.Keys.Except(copy.Keys).Select(path => "missing in .claude/skills: " + path)
            .Concat(copy.Keys.Except(original.Keys).Select(path => "only in .claude/skills: " + path))
            .Concat(original.Keys.Intersect(copy.Keys).Where(path => original[path] != copy[path])
                .Select(path => "differs: " + path))
            .ToList();
        Assert.True(problems.Count == 0,
            "Edit .agents/skills and copy it to .claude/skills (" + string.Join("; ", problems) + ")");
    }

    private static Dictionary<string, string> Files(string directory) =>
        Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).ToDictionary(
                path => Path.GetRelativePath(directory, path).Replace('\\', '/'),
                path => File.ReadAllText(path).Replace("\r\n", "\n"))
            : new Dictionary<string, string>();
}
