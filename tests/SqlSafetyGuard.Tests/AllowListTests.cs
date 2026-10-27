using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

public class AllowListTests
{
    [Theory]
    [InlineData("Staging_*", null, null, "dbo", "Staging_Import", true)]
    [InlineData("Staging_*", null, null, "etl", "staging_import", true)]
    [InlineData("dbo.Staging_*", null, null, null, "Staging_Import", false)]
    [InlineData("dbo.Staging_*", null, null, "etl", "Staging_Import", false)]
    [InlineData("[dbo].[Staging_*]", null, null, "dbo", "Staging_X", true)]
    [InlineData("etl.*", null, null, "ETL", "Anything", true)]
    [InlineData("dbo.Tmp?", null, null, "dbo", "Tmp1", true)]
    [InlineData("dbo.Tmp?", null, null, "dbo", "Tmp12", false)]
    [InlineData("Sandbox.dbo.*", null, "Sandbox", "dbo", "T", true)]
    [InlineData("Sandbox.dbo.*", null, null, "dbo", "T", false)]
    [InlineData("Sandbox.*.*", null, "sandbox", null, "T", false)]
    [InlineData("srv.Sandbox.dbo.*", "srv", "Sandbox", "dbo", "T", true)]
    [InlineData("Orders", null, null, "dbo", "Orders_Backup", false)]
    [InlineData("dbo.[ Safe]", null, null, "dbo", "Safe", false)]
    [InlineData("dbo.[Safe ]", null, null, "dbo", "Safe", false)]
    [InlineData("dbo.[Safe\n]", null, null, "dbo", "Safe", false)]
    [InlineData(" dbo . [ Safe] ", null, null, "dbo", " Safe", true)]
    [InlineData("dbo.\"Safe \"", null, null, "dbo", "Safe ", true)]
    public void Matches(string pattern, string? server, string? database, string? schema, string name, bool expected)
    {
        var list = new AllowList(new[] { pattern });
        Assert.Equal(expected, list.Matches(server, database, schema, name));
    }

    [Fact]
    public void IgnoresBlankAndCommentLines()
    {
        var options = AnalyzerOptions.FromAllowListText("\r\n  -- comment\r\n# also comment\r\n dbo.Staging_* \r\n");
        Assert.Equal(new[] { "dbo.Staging_*" }, options.AllowList.Patterns);
    }

    [Fact]
    public void EmptyAllowList_MatchesNothing()
    {
        Assert.False(new AllowList(Array.Empty<string>()).Matches(null, null, "dbo", "x"));
    }
}

public class TextPositionTests
{
    [Theory]
    [InlineData("abc", 0, 1, 1)]
    [InlineData("abc", 2, 1, 3)]
    [InlineData("a\r\nb", 3, 2, 1)]
    [InlineData("a\nb\nc", 4, 3, 1)]
    [InlineData("a\rb", 2, 2, 1)]
    public void FromOffset(string text, int offset, int line, int column)
    {
        TextPosition.FromOffset(text, offset, out var l, out var c);
        Assert.Equal(line, l);
        Assert.Equal(column, c);
    }
}
