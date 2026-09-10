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
/// Machine-readable error report for AI agents and CI tooling.
/// </summary>
public sealed class DiagnosticReport
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

    public List<DiagnosticItem> Errors { get; set; } = new();

    public string ToJson() => JsonSerializer.Serialize(this, s_options);

    public static DiagnosticReport FromException(Exception e, int exitCode = 1)
    {
        var report = new DiagnosticReport
        {
            Ok = false,
            ExitCode = exitCode,
        };
        Collect(e, report.Errors);
        if (report.Errors.Count == 0)
        {
            report.Errors.Add(new DiagnosticItem
            {
                Category = "unknown",
                Message = e.Message,
            });
        }
        return report;
    }

    public static DiagnosticReport Success() => new() { Ok = true, ExitCode = 0 };

    public static DiagnosticReport ValidationFailed(string message = null)
    {
        return new DiagnosticReport
        {
            Ok = false,
            ExitCode = 1,
            Errors =
            {
                new DiagnosticItem
                {
                    Category = "validation",
                    Code = "error.cli.validation_fail",
                    Message = message ?? MessageCatalog.Format("error.cli.validation_fail"),
                },
            },
        };
    }

    private static void Collect(Exception e, List<DiagnosticItem> items)
    {
        if (e == null)
        {
            return;
        }

        if (TryExtractDataCreateException(e, out var dce))
        {
            items.Add(new DiagnosticItem
            {
                Category = "data",
                Code = "error.data.parse_failed",
                Message = dce.OriginErrorMsg,
                File = dce.OriginDataLocation,
                Location = dce.DataLocationInFile,
                FieldPath = dce.VariableFullPathStr,
            });
            return;
        }

        if (e is LubanException le)
        {
            items.Add(new DiagnosticItem
            {
                Category = GuessCategory(le.MessageKey),
                Code = le.MessageKey,
                Message = le.Message,
                File = le.SchemaOrigin?.File,
                Location = le.SchemaOrigin?.Sheet,
                Args = le.Args is { Length: > 0 }
                    ? le.Args.Select(a => a?.ToString()).ToList()
                    : null,
            });
            if (e.InnerException != null && e.InnerException is not LubanException)
            {
                Collect(e.InnerException, items);
            }
            return;
        }

        if (e is AggregateException ae)
        {
            foreach (var inner in ae.InnerExceptions)
            {
                Collect(inner, items);
            }
            return;
        }

        if (e.InnerException != null)
        {
            Collect(e.InnerException, items);
            if (items.Count > 0)
            {
                return;
            }
        }

        items.Add(new DiagnosticItem
        {
            Category = "runtime",
            Message = e.Message,
        });
    }

    private static bool TryExtractDataCreateException(Exception e, out DataCreateException extract)
    {
        if (e is DataCreateException dce)
        {
            extract = dce;
            return true;
        }

        if (e is AggregateException ae)
        {
            foreach (var innerException in ae.InnerExceptions)
            {
                if (TryExtractDataCreateException(innerException, out extract))
                {
                    return true;
                }
            }
        }

        if (e.InnerException != null)
        {
            return TryExtractDataCreateException(e.InnerException, out extract);
        }

        extract = null;
        return false;
    }

    private static string GuessCategory(string messageKey)
    {
        if (string.IsNullOrEmpty(messageKey))
        {
            return "unknown";
        }
        if (messageKey.StartsWith("error.data.", StringComparison.Ordinal))
        {
            return "data";
        }
        if (messageKey.StartsWith("error.def.", StringComparison.Ordinal) ||
            messageKey.StartsWith("error.schema.", StringComparison.Ordinal))
        {
            return "schema";
        }
        if (messageKey.StartsWith("error.codegen.", StringComparison.Ordinal))
        {
            return "codegen";
        }
        if (messageKey.StartsWith("error.cli.", StringComparison.Ordinal))
        {
            return "cli";
        }
        if (messageKey.Contains("valid", StringComparison.OrdinalIgnoreCase))
        {
            return "validation";
        }
        return "other";
    }
}

public sealed class DiagnosticItem
{
    /// <summary>schema | data | validation | codegen | cli | runtime | other | unknown</summary>
    public string Category { get; set; }

    /// <summary>Stable message key when available, e.g. error.def.target.invalid</summary>
    public string Code { get; set; }

    public string Message { get; set; }

    public string File { get; set; }

    public string Location { get; set; }

    public string FieldPath { get; set; }

    public List<string> Args { get; set; }
}
