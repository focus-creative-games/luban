// Copyright 2025 Code Philosophy
//
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;

namespace Luban.Mcp;

[McpServerToolType]
public static class LubanTools
{
    private static readonly JsonSerializerOptions s_json = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    [McpServerTool, Description("List tables from a Luban project by exporting schema-json. Requires LUBAN_DLL (path to Luban.dll) or lubanDll argument.")]
    public static async Task<string> ListTables(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Export target name, e.g. client/server/all")] string target = "all",
        [Description("Optional override path to Luban.dll")] string? lubanDll = null,
        CancellationToken cancellationToken = default)
    {
        var schema = await ExportSchemaAsync(conf, target, lubanDll, cancellationToken);
        using var doc = JsonDocument.Parse(schema);
        var tables = doc.RootElement.GetProperty("tables");
        var list = new List<object>();
        foreach (var t in tables.EnumerateArray())
        {
            list.Add(new
            {
                fullName = t.GetProperty("fullName").GetString(),
                valueType = t.TryGetProperty("valueType", out var vt) ? vt.GetString() : null,
                mode = t.TryGetProperty("mode", out var m) ? m.GetString() : null,
                index = t.TryGetProperty("index", out var idx) ? idx.GetString() : null,
                comment = t.TryGetProperty("comment", out var c) && c.ValueKind != JsonValueKind.Null ? c.GetString() : null,
            });
        }
        return JsonSerializer.Serialize(list, s_json);
    }

    [McpServerTool, Description("Get compiled Luban schema as JSON (tables/beans/enums). Optionally filter by full name substring.")]
    public static async Task<string> GetSchema(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Export target name")] string target = "all",
        [Description("Optional filter: table/bean/enum fullName contains this string")] string? nameFilter = null,
        [Description("Optional override path to Luban.dll")] string? lubanDll = null,
        CancellationToken cancellationToken = default)
    {
        var schema = await ExportSchemaAsync(conf, target, lubanDll, cancellationToken);
        if (string.IsNullOrWhiteSpace(nameFilter))
        {
            return schema;
        }

        using var doc = JsonDocument.Parse(schema);
        var root = doc.RootElement;
        var filter = nameFilter.Trim();
        bool Match(JsonElement el) =>
            el.TryGetProperty("fullName", out var fn) &&
            (fn.GetString()?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false);

        var filtered = new
        {
            version = root.GetProperty("version").GetInt32(),
            target = root.GetProperty("target").GetString(),
            topModule = root.TryGetProperty("topModule", out var tm) ? tm.GetString() : null,
            tables = FilterArray(root, "tables", Match),
            beans = FilterArray(root, "beans", Match),
            enums = FilterArray(root, "enums", Match),
        };
        return JsonSerializer.Serialize(filtered, s_json);
    }

    [McpServerTool, Description("Validate Luban config/data without writing outputs (-f --strict --errorFormat json -x outputSaver=null).")]
    public static async Task<string> Validate(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Export target name")] string target = "all",
        [Description("Extra CLI args, space-separated, e.g. -x pathValidator.rootDir=...")] string? extraArgs = null,
        [Description("Optional override path to Luban.dll")] string? lubanDll = null,
        CancellationToken cancellationToken = default)
    {
        var args = new List<string>
        {
            "--conf", conf,
            "-t", target,
            "-f",
            "--strict",
            "--errorFormat", "json",
            "-x", "outputSaver=null",
        };
        if (!string.IsNullOrWhiteSpace(extraArgs))
        {
            args.AddRange(SplitArgs(extraArgs));
        }
        return await RunLubanAsync(args, lubanDll, cancellationToken);
    }

    [McpServerTool, Description("Run Luban generation with arbitrary CLI arguments (after Luban.dll). Prefer --errorFormat json for parseable errors.")]
    public static async Task<string> Generate(
        [Description("CLI args after Luban.dll, e.g. --conf path -t client -c cs-bin -d bin -x outputCodeDir=...")] string args,
        [Description("Optional override path to Luban.dll")] string? lubanDll = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            return JsonSerializer.Serialize(new { ok = false, message = "args is required" }, s_json);
        }
        var list = SplitArgs(args).ToList();
        if (!list.Any(a => string.Equals(a, "--errorFormat", StringComparison.OrdinalIgnoreCase)))
        {
            list.Add("--errorFormat");
            list.Add("json");
        }
        return await RunLubanAsync(list, lubanDll, cancellationToken);
    }

    [McpServerTool, Description("Search Luban documentation markdown files. Set LUBAN_DOC to docs root (folder containing intro.md) or pass docsRoot.")]
    public static string SearchDocs(
        [Description("Search keywords (space-separated, all must match case-insensitive)")] string query,
        [Description("Optional docs root override")] string? docsRoot = null,
        [Description("Max results")] int limit = 10)
    {
        var root = ResolveDocsRoot(docsRoot);
        if (root == null || !Directory.Exists(root))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                message = "Docs root not found. Set env LUBAN_DOC to luban-doc/docs or pass docsRoot.",
            }, s_json);
        }

        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (terms.Length == 0)
        {
            return JsonSerializer.Serialize(new { ok = false, message = "query is empty" }, s_json);
        }

        var results = new List<object>();
        foreach (var file in Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}_redirects{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            {
                continue;
            }
            string text;
            try
            {
                text = File.ReadAllText(file);
            }
            catch
            {
                continue;
            }

            if (!terms.All(t => text.Contains(t, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var rel = Path.GetRelativePath(root, file).Replace('\\', '/');
            var snippet = ExtractSnippet(text, terms[0], 240);
            results.Add(new { path = rel, title = ExtractTitle(text, rel), snippet });
            if (results.Count >= Math.Clamp(limit, 1, 50))
            {
                break;
            }
        }

        return JsonSerializer.Serialize(new { ok = true, docsRoot = root, count = results.Count, results }, s_json);
    }

    private static async Task<string> ExportSchemaAsync(string conf, string target, string? lubanDll, CancellationToken ct)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "luban-mcp-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var args = new List<string>
            {
                "--conf", conf,
                "-t", target,
                "-c", "schema-json",
                "--errorFormat", "json",
                "-x", $"outputCodeDir={tempDir}",
            };
            var run = await RunLubanAsync(args, lubanDll, ct);
            var schemaPath = Path.Combine(tempDir, "schema.json");
            if (!File.Exists(schemaPath))
            {
                return JsonSerializer.Serialize(new
                {
                    ok = false,
                    message = "schema.json was not produced",
                    lubanOutput = run,
                }, s_json);
            }
            return await File.ReadAllTextAsync(schemaPath, ct);
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { /* ignore */ }
        }
    }

    private static async Task<string> RunLubanAsync(IReadOnlyList<string> args, string? lubanDll, CancellationToken ct)
    {
        var dll = ResolveLubanDll(lubanDll);
        if (dll == null)
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                message = "Luban.dll not found. Set env LUBAN_DLL or pass lubanDll.",
            }, s_json);
        }

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        psi.ArgumentList.Add(dll);
        foreach (var a in args)
        {
            psi.ArgumentList.Add(a);
        }

        using var proc = new Process { StartInfo = psi };
        proc.Start();
        var stdoutTask = proc.StandardOutput.ReadToEndAsync(ct);
        var stderrTask = proc.StandardError.ReadToEndAsync(ct);
        await proc.WaitForExitAsync(ct);
        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        var reportJson = ExtractLastJsonObject(stderr) ?? ExtractLastJsonObject(stdout);
        return JsonSerializer.Serialize(new
        {
            ok = proc.ExitCode == 0,
            exitCode = proc.ExitCode,
            report = reportJson != null ? TryParseJson(reportJson) : null,
            stderr,
            stdout,
        }, s_json);
    }

    private static string? ResolveLubanDll(string? overridePath)
    {
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }
        var env = Environment.GetEnvironmentVariable("LUBAN_DLL");
        if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
        {
            return Path.GetFullPath(env);
        }

        // Dev fallback: sibling build output
        var candidates = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Luban", "bin", "Release", "net8.0", "Luban.dll")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Luban", "bin", "Debug", "net8.0", "Luban.dll")),
        };
        return candidates.FirstOrDefault(File.Exists);
    }

    private static string? ResolveDocsRoot(string? overridePath)
    {
        if (!string.IsNullOrWhiteSpace(overridePath) && Directory.Exists(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }
        var env = Environment.GetEnvironmentVariable("LUBAN_DOC");
        if (!string.IsNullOrWhiteSpace(env) && Directory.Exists(env))
        {
            return Path.GetFullPath(env);
        }
        var sibling = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "luban-doc", "docs"));
        return Directory.Exists(sibling) ? sibling : null;
    }

    private static List<JsonElement> FilterArray(JsonElement root, string name, Func<JsonElement, bool> pred)
    {
        var list = new List<JsonElement>();
        if (!root.TryGetProperty(name, out var arr) || arr.ValueKind != JsonValueKind.Array)
        {
            return list;
        }
        foreach (var el in arr.EnumerateArray())
        {
            if (pred(el))
            {
                list.Add(el.Clone());
            }
        }
        return list;
    }

    private static IEnumerable<string> SplitArgs(string args)
    {
        var rx = new Regex("\"([^\"]*)\"|(\\S+)");
        foreach (Match m in rx.Matches(args))
        {
            yield return m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
        }
    }

    private static string? ExtractLastJsonObject(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }
        int end = text.LastIndexOf('}');
        if (end < 0)
        {
            return null;
        }
        int depth = 0;
        for (int i = end; i >= 0; i--)
        {
            if (text[i] == '}')
            {
                depth++;
            }
            else if (text[i] == '{')
            {
                depth--;
                if (depth == 0)
                {
                    var candidate = text[i..(end + 1)];
                    try
                    {
                        using var _ = JsonDocument.Parse(candidate);
                        return candidate;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
        return null;
    }

    private static object? TryParseJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<JsonElement>(json);
        }
        catch
        {
            return json;
        }
    }

    private static string ExtractTitle(string text, string fallback)
    {
        foreach (var line in text.Split('\n'))
        {
            var t = line.Trim();
            if (t.StartsWith("# "))
            {
                return t[2..].Trim();
            }
        }
        return fallback;
    }

    private static string ExtractSnippet(string text, string term, int maxLen)
    {
        var idx = text.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            idx = 0;
        }
        var start = Math.Max(0, idx - 40);
        var len = Math.Min(maxLen, text.Length - start);
        var snippet = text.Substring(start, len).Replace('\r', ' ').Replace('\n', ' ');
        return snippet;
    }
}
