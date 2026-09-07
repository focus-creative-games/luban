// Copyright 2025 Code Philosophy

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace Luban.Tests.Helpers;

public static class GoldenAssert
{
    private static readonly HashSet<string> s_textExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".json", ".xml", ".yaml", ".yml", ".lua", ".txt", ".cs", ".proto", ".fbs",
    };

    public static void AssertDirectoryMatches(string actualDir, string goldenRelativeDir)
    {
        var goldenDir = TestPaths.Golden(goldenRelativeDir);
        Assert.True(Directory.Exists(actualDir), $"Actual output directory missing: {actualDir}");

        if (TestPaths.UpdateGolden)
        {
            if (Directory.Exists(goldenDir))
            {
                Directory.Delete(goldenDir, recursive: true);
            }

            CopyDirectory(actualDir, goldenDir);
            return;
        }

        Assert.True(Directory.Exists(goldenDir),
            $"Golden directory missing: {goldenDir}. Run with LUBAN_UPDATE_GOLDEN=1 to create it.");

        var actualFiles = EnumerateRelativeFiles(actualDir);
        var goldenFiles = EnumerateRelativeFiles(goldenDir);

        var missing = goldenFiles.Except(actualFiles, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        var unexpected = actualFiles.Except(goldenFiles, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        Assert.True(missing.Count == 0 && unexpected.Count == 0,
            $"File set mismatch under {goldenRelativeDir}. Missing: [{string.Join(", ", missing)}] Unexpected: [{string.Join(", ", unexpected)}]");

        foreach (var relative in goldenFiles.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            var actualPath = Path.Combine(actualDir, relative);
            var goldenPath = Path.Combine(goldenDir, relative);
            AssertFileMatches(actualPath, goldenPath, relative);
        }
    }

    public static void AssertFileMatches(string actualPath, string goldenPath, string displayName)
    {
        var ext = Path.GetExtension(actualPath);
        if (s_textExtensions.Contains(ext))
        {
            var actual = NormalizeText(File.ReadAllText(actualPath, Encoding.UTF8));
            var expected = NormalizeText(File.ReadAllText(goldenPath, Encoding.UTF8));
            if (string.Equals(ext, ".json", StringComparison.OrdinalIgnoreCase))
            {
                actual = NormalizeJson(actual);
                expected = NormalizeJson(expected);
            }

            Assert.True(string.Equals(actual, expected, StringComparison.Ordinal),
                $"Golden mismatch: {displayName}\n--- actual ---\n{Truncate(actual)}\n--- expected ---\n{Truncate(expected)}");
            return;
        }

        var actualHash = HashFile(actualPath);
        var expectedHash = HashFile(goldenPath);
        Assert.True(string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase),
            $"Binary golden hash mismatch: {displayName}\nactual={actualHash}\nexpected={expectedHash}");
    }

    public static string NormalizeText(string text)
    {
        text = text.Replace("\r\n", "\n").Replace('\r', '\n');
        // Drop trailing spaces on each line for stable diffs across writers.
        text = Regex.Replace(text, "[ \t]+\n", "\n");
        return text.TrimEnd() + "\n";
    }

    public static string NormalizeJson(string text)
    {
        using var doc = JsonDocument.Parse(text, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });
        return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
        {
            WriteIndented = true,
        }) + "\n";
    }

    private static string HashFile(string path)
    {
        using var stream = File.OpenRead(path);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    private static List<string> EnumerateRelativeFiles(string root)
    {
        return Directory.GetFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(root, f).Replace('\\', '/'))
            .Where(f => !f.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(source, destination));
        }

        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var dest = file.Replace(source, destination);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, overwrite: true);
        }
    }

    private static string Truncate(string text, int max = 2000)
        => text.Length <= max ? text : text[..max] + "\n... (truncated)";
}
