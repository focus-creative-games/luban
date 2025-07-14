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
