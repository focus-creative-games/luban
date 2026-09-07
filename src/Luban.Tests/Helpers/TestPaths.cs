// Copyright 2025 Code Philosophy

namespace Luban.Tests.Helpers;

public static class TestPaths
{
    private static readonly Lazy<string> s_repoRoot = new(FindRepoRoot);

    public static string RepoRoot => s_repoRoot.Value;

    public static string FixturesRoot => Path.Combine(RepoRoot, "tests", "fixtures");

    public static string GoldenRoot => Path.Combine(RepoRoot, "tests", "golden");

    public static string RuntimeSupportRoot
    {
        get
        {
            var fromOutput = Path.Combine(AppContext.BaseDirectory, "RuntimeSupport");
            if (Directory.Exists(fromOutput))
            {
                return fromOutput;
            }

            return Path.Combine(RepoRoot, "src", "Luban.Tests", "RuntimeSupport");
        }
    }

    public static string Fixture(string relativePath) => Path.Combine(FixturesRoot, relativePath);

    public static string FixtureConf(string fixtureName) => Path.Combine(FixturesRoot, fixtureName, "luban.conf");

    public static string Golden(string relativePath) => Path.Combine(GoldenRoot, relativePath);

    public static bool UpdateGolden =>
        string.Equals(Environment.GetEnvironmentVariable("LUBAN_UPDATE_GOLDEN"), "1", StringComparison.Ordinal)
        || string.Equals(Environment.GetEnvironmentVariable("LUBAN_UPDATE_GOLDEN"), "true", StringComparison.OrdinalIgnoreCase);

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "src", "Luban.sln"))
                && Directory.Exists(Path.Combine(dir.FullName, "tests")))
            {
                return dir.FullName;
            }

            // Also accept walking up from source tree before tests/ exists during bootstrap.
            if (File.Exists(Path.Combine(dir.FullName, "src", "Luban.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException($"Cannot locate Luban repo root from {AppContext.BaseDirectory}");
    }
}
