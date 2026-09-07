// Copyright 2026 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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

    [McpServerTool, Description("List Luban tables via Luban.Agent. Prefers LUBAN_AGENT_DLL.")]
    public static async Task<string> ListTables(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Export target name, e.g. client/server/all")] string target = "all",
        [Description("Optional path to Luban.Agent.dll")] string? agentDll = null,
        CancellationToken cancellationToken = default)
    {
        return await RunAgentAsync(
            ["list-tables", "--conf", conf, "-t", target],
            agentDll,
            cancellationToken);
    }

    [McpServerTool, Description("Get compiled Luban schema JSON via Luban.Agent (optional name filter).")]
    public static async Task<string> GetSchema(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Export target name")] string target = "all",
        [Description("Optional filter: fullName contains this string")] string? nameFilter = null,
        [Description("Optional path to Luban.Agent.dll")] string? agentDll = null,
        CancellationToken cancellationToken = default)
    {
        var args = new List<string> { "schema", "--conf", conf, "-t", target };
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            args.Add("--name");
            args.Add(nameFilter);
        }
        return await RunAgentAsync(args, agentDll, cancellationToken);
    }

    [McpServerTool, Description("Describe a table/bean/enum by name via Luban.Agent.")]
    public static async Task<string> Describe(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Table/bean/enum name or fullName substring")] string name,
        [Description("Export target name")] string target = "all",
        [Description("Optional path to Luban.Agent.dll")] string? agentDll = null,
        CancellationToken cancellationToken = default)
    {
        return await RunAgentAsync(
            ["describe", "--conf", conf, "-t", target, "--name", name],
            agentDll,
            cancellationToken);
    }

    [McpServerTool, Description("Validate Luban config/data via Luban.Agent (no file output).")]
    public static async Task<string> Validate(
        [Description("Absolute path to luban.conf")] string conf,
        [Description("Export target name")] string target = "all",
        [Description("Extra CLI args, space-separated, e.g. -x pathValidator.rootDir=...")] string? extraArgs = null,
        [Description("Optional path to Luban.Agent.dll")] string? agentDll = null,
        CancellationToken cancellationToken = default)
    {
        var args = new List<string> { "validate", "--conf", conf, "-t", target };
        if (!string.IsNullOrWhiteSpace(extraArgs))
        {
            args.AddRange(SplitArgs(extraArgs));
        }
        return await RunAgentAsync(args, agentDll, cancellationToken);
    }

    [McpServerTool, Description("Run main Luban generation (Luban.dll). Prefer --errorFormat json.")]
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
        return await RunDotnetDllAsync(ResolveLubanDll(lubanDll), list, "Luban.dll not found. Set env LUBAN_DLL.", cancellationToken);
    }

    [McpServerTool, Description("Search Luban documentation markdown files. Set LUBAN_DOC to docs root or pass docsRoot.")]
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

    private static Task<string> RunAgentAsync(IReadOnlyList<string> args, string? agentDll, CancellationToken ct)
        => RunDotnetDllAsync(ResolveAgentDll(agentDll), args, "Luban.Agent.dll not found. Set env LUBAN_AGENT_DLL.", ct);

    private static async Task<string> RunDotnetDllAsync(string? dll, IReadOnlyList<string> args, string missingMessage, CancellationToken ct)
    {
        if (dll == null)
        {
            return JsonSerializer.Serialize(new { ok = false, message = missingMessage }, s_json);
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

        var reportJson = ExtractLastJsonObject(stdout) ?? ExtractLastJsonObject(stderr);
        return JsonSerializer.Serialize(new
        {
            ok = proc.ExitCode == 0,
            exitCode = proc.ExitCode,
            report = reportJson != null ? TryParseJson(reportJson) : null,
            stderr,
            stdout,
        }, s_json);
    }

    private static string? ResolveAgentDll(string? overridePath)
    {
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }
        var env = Environment.GetEnvironmentVariable("LUBAN_AGENT_DLL");
        if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
        {
            return Path.GetFullPath(env);
        }
        var candidates = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Luban.Agent", "bin", "Release", "net8.0", "Luban.Agent.dll")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Luban.Agent", "bin", "Debug", "net8.0", "Luban.Agent.dll")),
        };
        return candidates.FirstOrDefault(File.Exists);
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
        return text.Substring(start, len).Replace('\r', ' ').Replace('\n', ' ');
    }
}
