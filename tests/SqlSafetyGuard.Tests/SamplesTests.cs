using System.Text.RegularExpressions;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

/// <summary>Checks every "-- #n" example in samples/01-try-these.sql against its "-- expect:" verdict.</summary>
public class SamplesTests
{
    public static TheoryData<string, string, Verdict> Examples()
    {
        var data = new TheoryData<string, string, Verdict>();
        var text = File.ReadAllText(Path.Combine(FindSamplesFolder(), "01-try-these.sql"));
        var blocks = Regex.Split(text, @"(?m)^(?=-- #\d+)").Where(b => b.StartsWith("-- #", StringComparison.Ordinal));
        foreach (var block in blocks)
        {
            var title = block.Substring(0, block.IndexOfAny(new[] { '\r', '\n' }));
            var expect = Regex.Match(block, @"(?m)^-- expect: (\w+)");
            data.Add(title, block, Enum.Parse<Verdict>(expect.Groups[1].Value));
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void Example_HasExpectedVerdict(string title, string sql, Verdict expected)
    {
        var result = SqlSafetyAnalyzer.Analyze(sql);
        Assert.True(expected == result.Overall, $"{title}: expected {expected}, got {result.Overall}");
    }

    [Fact]
    public void AllowListExample_TurnsGreenWithPattern()
    {
        var result = SqlSafetyAnalyzer.Analyze("UPDATE dbo.Staging_Import SET X = 1;", new AnalyzerOptions(new[] { "dbo.Staging_*" }));
        Assert.Equal(Verdict.AllowListed, result.Overall);
    }

    [Fact]
    public void SetupScript_IsRed()
    {
        var sql = File.ReadAllText(Path.Combine(FindSamplesFolder(), "00-setup-sandbox.sql"));
        Assert.Equal(Verdict.RealWrite, SqlSafetyAnalyzer.Analyze(sql).Overall);
    }

    private static string FindSamplesFolder()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "samples");
            if (File.Exists(Path.Combine(candidate, "01-try-these.sql")))
                return candidate;
        }

        throw new DirectoryNotFoundException("samples folder not found");
    }
}
