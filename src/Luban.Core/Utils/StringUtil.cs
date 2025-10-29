// Copyright 2025 Code Philosophy
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

using System.Collections.Specialized;
using System.Text;

namespace Luban.Utils;

public static class StringUtil
{
    public static string CollectionToString<T>(IEnumerable<T> collection)
    {
        return string.Join(",", collection);
    }

    public static string RepeatString(string str, int count)
    {
        return count == 0 ? "" : string.Concat(Enumerable.Repeat(str, count));
    }

    public static string RepeatSpaceAsTab(int count)
    {
        return count == 0 ? "" : string.Concat(Enumerable.Repeat("    ", count));
    }

    public static string GetLineEnding(string endings)
    {
        switch (endings)
        {
            case "":
                return Environment.NewLine;
            case "crlf":
                return "\r\n";
            case "lf":
                return "\n";
            case "cr":
                return "\r";
            default:
                throw new Exception($"unknown line ending: {endings}");
        }
    }

    public static string[] SplitStringWithEscape(string str, char separator)
    {
        char escapeChar = '\\';
        if (string.IsNullOrEmpty(str))
        {
            return Array.Empty<string>();
        }
        var result = new List<string>();
        var current = new StringBuilder();
        bool isEscaped = false;
        foreach (char c in str)
        {
            if (isEscaped)
            {
                if (c != separator)
                {
                    // Otherwise, treat the escape character as a literal character
                    current.Append(escapeChar);
                }
                current.Append(c);
                isEscaped = false;
            }
            else if (c == escapeChar)
            {
                isEscaped = true;
            }
            else if (c == separator)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        if (current.Length > 0)
        {
            result.Add(current.ToString());
        }
        return result.ToArray();
    }

    public static string UnEscapeString(string str)
    {
        char escapeChar = '\\';
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        var current = new StringBuilder();
        bool isEscaped = false;
        foreach (char c in str)
        {
            if (isEscaped)
            {
                current.Append(c);
                isEscaped = false;
            }
            else if (c == escapeChar)
            {
                isEscaped = true;
            }
            else
            {
                current.Append(c);
            }
        }
        return current.ToString();
    }
}
