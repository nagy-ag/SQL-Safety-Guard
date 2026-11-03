using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class TestSourceIndexTests
{
    [Fact]
    public void IndexUsesAttributedDeclarationsRatherThanInvocationsOrComments()
    {
        const string source = """
            namespace Example;
            public class Outer {
              // [Fact] public void Fake() {}
              public class Inner {
                [Theory] public void Matches(string value) { Regex.Matches(value, "x"); }
                public void Helper() { Matches("text"); }
              }
            }
            """;
        var entry = Assert.Single(TestSourceIndex.Read(source, "Example.cs"));
        Assert.Equal("Example.Outer+Inner.Matches", entry.Name);
        Assert.Equal(5, entry.Line);
        Assert.Equal("Example.cs", entry.File);
        Assert.NotNull(Args.Parse(["--test-index", "sources"]).TestIndex);
    }
}
