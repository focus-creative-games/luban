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
            if (string.IsNullOrWhiteSpace(tags))
            {
                return null;
            }
            var am = new Dictionary<string, string>();
            foreach (var pair in tags.Split(s_attrSep))
            {
                int sepIndex = pair.IndexOfAny(s_attrKeyValueSep);
                if (sepIndex >= 0)
                {
                    am.Add(pair[..sepIndex].Trim(), pair[(sepIndex + 1)..].Trim());
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
                return (s, null);
            }
            else
            {
                return (s[..sepIndex], ParseAttrs(s[(sepIndex + 1)..]));
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
