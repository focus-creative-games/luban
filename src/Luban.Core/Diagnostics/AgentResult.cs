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

using Luban.DataLoader;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luban.Diagnostics;

/// <summary>
/// Stable process exit codes for agents and CI.
/// </summary>
public static class AgentExitCode
{
    public const int Ok = 0;
    public const int DataOrValidation = 1;
    public const int SchemaOrConfig = 2;
    public const int Usage = 3;
    public const int Internal = 4;

    public static int FromCategory(string category)
    {
        return category switch
        {
            "data" or "validation" => DataOrValidation,
            "schema" or "codegen" => SchemaOrConfig,
            "cli" => Usage,
            _ => Internal,
        };
    }

    public static int FromErrors(IReadOnlyList<DiagnosticItem> errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return Internal;
        }
        return FromCategory(errors[0].Category);
    }
}

/// <summary>
/// Unified JSON envelope for --agent / machine-readable CLI modes.
/// Written to stdout when --agent is set.
/// </summary>
public sealed class AgentResult
{
    public const int FormatVersion = 1;

    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public int Version { get; set; } = FormatVersion;

    public bool Ok { get; set; }

    public int ExitCode { get; set; }

    public string Command { get; set; }

    public List<DiagnosticItem> Errors { get; set; } = new();

    public object Result { get; set; }

    public string ToJson() => JsonSerializer.Serialize(this, s_options);

    public static AgentResult Success(string command, object result = null)
        => new() { Ok = true, ExitCode = AgentExitCode.Ok, Command = command, Result = result };

    public static AgentResult FromReport(string command, DiagnosticReport report)
    {
        return new AgentResult
        {
            Ok = report.Ok,
            ExitCode = report.ExitCode,
            Command = command,
            Errors = report.Errors ?? new List<DiagnosticItem>(),
        };
    }

    public static AgentResult Fail(string command, Exception e)
    {
        var report = DiagnosticReport.FromException(e);
        report.ExitCode = AgentExitCode.FromErrors(report.Errors);
        return FromReport(command, report);
    }

    public static AgentResult Usage(string message)
    {
        return new AgentResult
        {
            Ok = false,
            ExitCode = AgentExitCode.Usage,
            Command = "usage",
            Errors =
            {
                new DiagnosticItem { Category = "cli", Code = "error.cli.usage", Message = message },
            },
        };
    }
}
