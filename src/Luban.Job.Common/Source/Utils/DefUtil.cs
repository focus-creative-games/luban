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

        public static (string, Dictionary<string, string>) ParseType(string s)
        {
            int sepIndex = s.IndexOfAny(s_attrSep);
            if (sepIndex < 0)
            {
                return (s, new Dictionary<string, string>());
            }
            else
            {
#if !LUBAN_LITE
                return (s[..sepIndex], ParseAttrs(s[(sepIndex + 1)..]));
#else
                return (s.Substring(0, sepIndex), ParseAttrs(s.Substring(sepIndex + 1)));
#endif
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
    }
}
