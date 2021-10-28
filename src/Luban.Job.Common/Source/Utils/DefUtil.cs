using Bright.Collections;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Utils
{
    public class DefUtil
    {
        private readonly static char[] s_attrSep = new char[] { '|', '#', '&' };

        private readonly static char[] s_attrKeyValueSep = new char[] { '=', ':' };

        public static Dictionary<string, string> ParseAttrs(string tags)
        {
            var am = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(tags))
            {
                return am;
            }
            foreach (var pair in tags.Split(s_attrSep))
            {
                int sepIndex = pair.IndexOfAny(s_attrKeyValueSep);
                if (sepIndex >= 0)
                {
#if !LUBAN_LITE
                    am.Add(pair[..sepIndex].Trim(), pair[(sepIndex + 1)..].Trim());
#else
                    am.Add(pair.Substring(0, sepIndex).Trim(), pair.Substring(sepIndex + 1).Trim());
#endif
                }
                else
                {
                    am.Add(pair.Trim(), pair.Trim());
                }
            }
            return am;
        }

        public static int IndexOfIncludeBrace(string s, char sep)
        {
            int braceDepth = 0;
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '(' || c == '[' || c == '{')
                {
                    ++braceDepth;
                }
                else if (c == ')' || c == ')' || c == '}')
                {
                    --braceDepth;
                }

                if (braceDepth == 0 && (c == sep))
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
                if (rawType[rawType.Length - 1] == ')')
                {
                    rawType = rawType.Substring(1, rawType.Length - 2);
                }
                else
                {
                    throw new Exception($"type:{rawType} brace not match");
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
                    else if (c == ')' || c == ')' || c == '}')
                    {
                        --braceDepth;
                    }

                    if (braceDepth == 0 && (c == '#' || c == '&' || c == '|'))
                    {
                        return (s.Substring(0, i), ParseAttrs(s.Substring(i + 1)));
                    }
                }
                return (s, new Dictionary<string, string>());
            }
        }

        public static bool ParseOrientation(string value)
        {
            switch (value.Trim())
            {
                case "":
                case "r":
                case "row": return true;
                case "c":
                case "column": return false;
                default:
                {
                    throw new Exception($"orientation 属性值只能为row|r|column|c");
                }
            }
        }

        public static bool IsNormalFieldName(string name)
        {
            return !name.StartsWith("__") && !name.StartsWith("#");
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
    }
}
