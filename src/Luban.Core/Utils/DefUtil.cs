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

﻿using System.Text;

namespace Luban.Utils;

public static class DefUtil
{
    private static readonly char[] s_attrSep = new char[] { '#' };

    private static readonly char[] s_attrKeyValueSep = new char[] { '=', ':' };

    private static void AddAttr(Dictionary<string, string> attrs, string rawPair)
    {
        var pair = TrimBracePairs(rawPair);
        int sepIndex = pair.IndexOfAny(s_attrKeyValueSep);
        string key;
        string value;
        if (sepIndex >= 0)
        {
            key = pair.Substring(0, sepIndex).Trim();
            value = pair.Substring(sepIndex + 1).Trim();
        }
        else
        {
            key = value = pair.Trim();
        }
        attrs.Add(key, value);
    }

    public static Dictionary<string, string> ParseAttrs(string tags)
    {
        var am = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(tags))
        {
            return am;
        }

        int braceDepth = 0;
        var buf = new StringBuilder();
        bool inEscape = false;
        for (int i = 0; i < tags.Length; i++)
        {
            var c = tags[i];
            if (inEscape)
            {
                inEscape = false;
                buf.Append(c);
                continue;
            }
            if (c == '\\')
            {
                inEscape = true;
                continue;
            }
            if (c == '(' || c == '[' || c == '{')
            {
                ++braceDepth;
            }
            else if (c == ')' || c == ']' || c == '}')
            {
                --braceDepth;
            }

            if (braceDepth == 0 && c == '#')
            {
                string rawPair = buf.ToString();
                buf.Clear();
                AddAttr(am, rawPair);
            }
            else
            {
                buf.Append(c);
            }
        }
        if (braceDepth != 0)
        {
            throw new Exception($"非法tags:{tags}");
        }
        if (buf.Length > 0)
        {
            AddAttr(am, buf.ToString());
        }
        return am;
    }

    public static int IndexOfBaseTypeEnd(string s)
    {
        int braceDepth = 0;
        int firstSharpIndex = -1;// '#'
        for (int i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '(' || c == '[' || c == '{')
            {
                ++braceDepth;
            }
            else if (c == ')' || c == ']' || c == '}')
            {
                --braceDepth;
            }
            if (c == '#' && firstSharpIndex == -1)
            {
                firstSharpIndex = i;
            }

            if (braceDepth == 0 && (c == ',' || c == ';'))
            {
                var strContainBaseType = firstSharpIndex > 0 ? s.Substring(0, firstSharpIndex) : s.Substring(0, i);
                strContainBaseType = strContainBaseType.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "");

                if (strContainBaseType == "array" || strContainBaseType == "list" || strContainBaseType == "set" || strContainBaseType == "map")
                {
                    return i;
                }
                else
                {
                    return -1;
                }
            }
        }
        return -1;
    }

    public static int IndexOfElementTypeSep(string s)
    {
        int braceDepth = 0;
        int firstSharpIndex = -1;// '#'
        for (int i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '(' || c == '[' || c == '{')
            {
                ++braceDepth;
            }
            else if (c == ')' || c == ']' || c == '}')
            {
                --braceDepth;
            }
            if (c == '#' && firstSharpIndex == -1)
            {
                firstSharpIndex = i;
            }

            if (braceDepth == 0 && (c == ',' || c == ';'))
            {
                return i;
            }
        }
        return -1;
    }

    public static string TrimBracePairs(string rawType)
    {
        while (rawType.Length > 0 && rawType[0] == '(')
        {
            int braceDepth = 0;
            int level1Left = -1;
            int level1Right = -1;
            for (int i = 0; i < rawType.Length; i++)
            {
                char c = rawType[i];
                if (c == '(' || c == '[' || c == '{')
                {
                    braceDepth++;
                    if (level1Left < 0)
                    {
                        level1Left = i;
                    }
                }
                if (c == ')' || c == ']' || c == '}')
                {
                    braceDepth--;
                    if (level1Right < 0 && braceDepth == 0 && c == ')')
                    {
                        level1Right = i;
                        break;
                    }
                }
            }
            if (level1Left >= 0 && level1Right == rawType.Length - 1)
            {
                rawType = rawType.Substring(1, rawType.Length - 2);
            }
            else
            {
                break;
            }
        }
        return rawType;
    }
    public static string TrimBracePairs2(string rawType, bool soft = false)
    {
        while (rawType.Length > 0 && rawType[0] == '(')
        {
            if (rawType[rawType.Length - 1] == ')')
            {
                rawType = rawType.Substring(1, rawType.Length - 2);
            }
            else
            {
                if (soft)
                {
                    return rawType;
                }
                else
                {
                    throw new Exception($"type:{rawType} brace not match");
                }
            }
        }
        return rawType;
    }

    public static (string, Dictionary<string, string>) ParseType(string s)
    {
        int sepIndex = s.IndexOfAny(s_attrSep);
        if (sepIndex < 0)
        {
            return (s, new Dictionary<string, string>());
        }
        else
        {
            int braceDepth = 0;
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '(' || c == '[' || c == '{')
                {
                    ++braceDepth;
                }
                else if (c == ')' || c == ']' || c == '}')
                {
                    --braceDepth;
                }

                if (braceDepth == 0 && (c == '#'))
                {
                    return (s.Substring(0, i), ParseAttrs(s.Substring(i + 1)));
                }
            }
            return (s, new Dictionary<string, string>());
        }
    }

    public static (string, Dictionary<string, string>) ParseTypeAndVaildAttrs(string s)
    {
        var (typeStr, attrs) = ParseType(s);

        if (attrs.ContainsKey("group"))
        {
            throw new Exception("group为保留属性,只能用于table或var定义,是否用错? 如在excel中请使用&group=xxx");
        }

        if (attrs.ContainsKey("seq"))
        {
            throw new Exception("字段切割应该用'sep'，而不是'seq',请检查是否拼写错误");
        }

        return (typeStr, attrs);
    }

    public static bool ParseOrientation(string value)
    {
        switch (value.Trim())
        {
            case "":
            case "r":
            case "row":
                return true;
            case "c":
            case "column":
                return false;
            default:
            {
                throw new Exception($"orientation 属性值只能为row|r|column|c");
            }
        }
    }

    public static bool IsNormalFieldName(string name)
    {
        return !name.StartsWith("__") && !name.StartsWith("#") && !name.StartsWith("$");
    }

    public static Dictionary<string, string> MergeTags(Dictionary<string, string> tags1, Dictionary<string, string> tags2)
    {
        if (tags2 != null && tags2.Count > 0)
        {
            if (tags1 != null)
            {
                if (tags1.Count == 0)
                {
                    return tags2;
                }
                else
                {
                    var result = new Dictionary<string, string>(tags1);
                    result.AddAll(tags2);
                    return result;
                }
            }
            else
            {
                return tags2;
            }
        }
        else
        {
            return tags1;
        }
    }

    public static List<string> ParseVariant(string s)
    {
        return s.Split(',', ';').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    // public static string EscapeCommentByCurrentLanguage(string comment)
    // {
    //     var curLan = DefAssembly.LocalAssebmly.CurrentLanguage;
    //     switch (curLan)
    //     {
    //         case ELanguage.INVALID: throw new Exception($"not set current language. can't get recommend naming convention name");
    //         case ELanguage.CS:
    //         case ELanguage.JAVA:
    //         case ELanguage.GO:
    //         case ELanguage.CPP:
    //         case ELanguage.LUA:
    //         case ELanguage.JAVASCRIPT:
    //         case ELanguage.TYPESCRIPT:
    //         case ELanguage.PYTHON:
    //         case ELanguage.RUST:
    //         case ELanguage.PROTOBUF:
    //         case ELanguage.GDSCRIPT:
    //             return System.Web.HttpUtility.HtmlEncode(comment).Replace("\n", "<br/>");
    //         default: throw new Exception($"unknown language:{curLan}");
    //     }
    // }
    //
    // public static ELanguage ParseLanguage(string lan)
    // {
    //     switch (lan.ToLower())
    //     {
    //         case "cs":
    //         case "c#":
    //         case "csharp": return ELanguage.CS;
    //         case "java": return ELanguage.JAVA;
    //         case "go":
    //         case "golang": return ELanguage.GO;
    //         case "cpp":
    //         case "c++": return ELanguage.CPP;
    //         case "lua": return ELanguage.LUA;
    //         case "js":
    //         case "javascript": return ELanguage.JAVASCRIPT;
    //         case "ts":
    //         case "typescript": return ELanguage.TYPESCRIPT;
    //         case "python": return ELanguage.PYTHON;
    //         case "rust": return ELanguage.RUST;
    //         case "pb":
    //         case "protobuf": return ELanguage.PROTOBUF;
    //         case "gdscript": return ELanguage.GDSCRIPT;
    //         default: throw new ArgumentException($"parse lan:'{lan}' fail");
    //     }
    // }
}
