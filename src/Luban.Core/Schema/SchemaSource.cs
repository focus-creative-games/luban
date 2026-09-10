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

using Luban.Pipeline;
using Luban.Utils;

namespace Luban.Schema;

/// <summary>
/// Where a schema type (table/bean/enum) was defined.
/// <see cref="File"/> is relative to the luban.conf directory when possible.
/// <see cref="Sheet"/> is set only for Excel/CSV-style definitions.
/// </summary>
public sealed class SchemaSource
{
    public string File { get; set; }

    public string Sheet { get; set; }

    /// <summary>Log-friendly form: <c>Sheet@file</c> or just <c>file</c>.</summary>
    public string Display =>
        string.IsNullOrEmpty(Sheet) ? File : $"{Sheet}@{File}";

    public static SchemaSource Create(string file, string sheet = null)
    {
        if (string.IsNullOrEmpty(file))
        {
            return null;
        }
        return new SchemaSource
        {
            File = Relativize(file),
            Sheet = string.IsNullOrEmpty(sheet) ? null : sheet,
        };
    }

    /// <summary>
    /// Parse <c>Sheet@path</c> (or plain path) and relativize against config dir.
    /// </summary>
    public static SchemaSource FromPath(string fileOrSheetUrl)
    {
        if (string.IsNullOrEmpty(fileOrSheetUrl))
        {
            return null;
        }
        var (file, sheet) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileOrSheetUrl));
        return Create(file, sheet);
    }

    private static string Relativize(string path)
    {
        path = FileUtil.Standardize(path);
        if (!Path.IsPathRooted(path))
        {
            return path;
        }

        string configDir = null;
        if (PipelineScope.HasCurrent)
        {
            configDir = GenerationContext.GlobalConf?.ConfigDir;
        }
        if (string.IsNullOrEmpty(configDir))
        {
            return path;
        }

        try
        {
            return FileUtil.Standardize(Path.GetRelativePath(configDir, path));
        }
        catch
        {
            return path;
        }
    }
}
