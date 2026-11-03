using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.Tests;

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
