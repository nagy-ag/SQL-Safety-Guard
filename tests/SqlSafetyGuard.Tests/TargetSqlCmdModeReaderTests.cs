using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.Tests;

public class TargetSqlCmdModeReaderTests
{
    private sealed class Current(bool mode) { public bool IsSqlCmdModeEnabled { get; set; } = mode; }
    private class LegacyBase(bool mode) { private bool IsSqlCmdMode => mode; }
    private sealed class Legacy(bool mode) : LegacyBase(mode);
    private sealed class Both(bool current, bool legacy)
    {
        public bool IsSqlCmdModeEnabled => current;
        public bool IsSqlCmdMode => legacy;
    }
    private sealed class Throws { public bool IsSqlCmdModeEnabled => throw new InvalidOperationException(); }
    private sealed class InvalidShape { public string IsSqlCmdModeEnabled => "False"; public bool IsSqlCmdMode => false; }
    private sealed class NullableShape { public bool? IsSqlCmdModeEnabled => false; }
    private sealed class BrokenAlias { public bool IsSqlCmdModeEnabled => false; public bool IsSqlCmdMode => throw new InvalidOperationException(); }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadsCurrentAndInheritedPrivateLegacyProperties(bool mode)
    {
        Assert.Equal(mode, TargetSqlCmdModeReader.TryRead(new Current(mode)));
        Assert.Equal(mode, TargetSqlCmdModeReader.TryRead(new Legacy(mode)));
        Assert.Equal(mode, TargetSqlCmdModeReader.TryRead(new Both(mode, mode)));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void ConflictingAliasesRemainUnknown(bool current, bool legacy) =>
        Assert.Null(TargetSqlCmdModeReader.TryRead(new Both(current, legacy)));

    [Fact]
    public void MissingMalformedOrThrowingStateRemainsUnknown()
    {
        foreach (var editor in new object[] { new object(), new Throws(), new InvalidShape(), new NullableShape(), new BrokenAlias() })
            Assert.Null(TargetSqlCmdModeReader.TryRead(editor));
        Assert.Null(TargetSqlCmdModeReader.TryRead(null!));
    }

    [Fact]
    public void OppositeEditorsAndStateChangesNeverShareCachedState()
    {
        var first = new Current(true);
        var second = new Current(false);
        Assert.True(TargetSqlCmdModeReader.TryRead(first));
        Assert.False(TargetSqlCmdModeReader.TryRead(second));
        Assert.True(TargetSqlCmdModeReader.TryRead(first));
        first.IsSqlCmdModeEnabled = false;
        second.IsSqlCmdModeEnabled = true;
        Assert.False(TargetSqlCmdModeReader.TryRead(first));
        Assert.True(TargetSqlCmdModeReader.TryRead(second));
    }
}
