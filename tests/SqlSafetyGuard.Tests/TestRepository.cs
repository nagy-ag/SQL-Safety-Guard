namespace SqlSafetyGuard.Tests;

internal static class TestRepository
{
    internal static string Root { get; } = FindRoot(AppContext.BaseDirectory);

    private static string FindRoot(string start)
    {
        for (var directory = new DirectoryInfo(start); directory != null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "SqlSafetyGuard.sln")) &&
                Directory.Exists(Path.Combine(directory.FullName, "src"))) return directory.FullName;
        throw new DirectoryNotFoundException("Run repository tests from a checkout containing SqlSafetyGuard.sln.");
    }
}
