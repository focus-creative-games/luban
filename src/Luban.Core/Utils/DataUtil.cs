using System.Text;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.Utils;

public static class DataUtil
{
    public static bool ParseBool(string s)
    {
        s = s.ToLower().Trim();
        switch (s)
        {
            case "true":
            case "1":
                return true;
            case "false":
            case "0":
                return false;
            default:
                throw new Exception($"{s} 不是 bool 类型的值 (true|1 或 false|0)");
        }
    }

    private static readonly string[] dateTimeFormats = new string[] {
        "yyyy-M-d HH:mm:ss", "yyyy-M-d HH:mm", "yyyy-M-d HH", "yyyy-M-d",
        //"yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH", "yyyy/MM/dd",
    };
    public static DDateTime CreateDateTime(string x)
    {
        DateTime dateTime = DateTime.ParseExact(x, dateTimeFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        //return new DDateTime(TimeZoneInfo.ConvertTime(dateTime, TimeZoneUtil.DefaultTimeZone, TimeZoneInfo.Utc));
        return new DDateTime(dateTime);
    }

    public static string FormatDateTime(DateTime datetime)
    {
        return datetime.ToString("yyyy-M-d HH:mm:ss");
    }

    public static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    public static string UnEscapeRawString(string s)
    {
        switch (s)
        {
            case "null":
                return null;
            case "\"\"":
                return string.Empty;
            default:
                return s;
        }
    }

    public static string EscapeString(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
    public static string UnEscapeString(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\\"", "\"");
    }

    public static string EscapeLuaStringWithQuote(string s)
    {
        if (!s.Contains('\"') && !s.Contains('\\') && !s.Contains('\n'))
        {
            return "\"" + s + "\"";
        }

        var multiEqaulChars = new StringBuilder();
        var result = new StringBuilder();
        var startIndex = s.EndsWith(']') ? 1 : 0;
        for (int i = startIndex; i < 100; i++)
        {
            if (i > 0)
            {
                multiEqaulChars.Append('=');
            }
            var multiEqualStr = multiEqaulChars.ToString();
            if (i == 0 || s.Contains(multiEqualStr))
            {
                if (s.Contains("[" + multiEqualStr + "[") || s.Contains("]" + multiEqualStr + "]"))
                {
                    continue;
                }
            }
            result.Clear();
            result.Append('[').Append(multiEqualStr).Append('[');
            result.Append(s);
            result.Append(']').Append(multiEqualStr).Append(']');
            return result.ToString();
        }
        throw new Exception($"too complex string:'{s}'");
    }

    //public static string EscapeStringWithQuote(string s)
    //{
    //    return "\"" + s.Replace("\\", "\\\\") + "\"";
    //}

    public static bool IsIgnoreTag(string tagName)
    {
        return tagName == "##";
    }

    public static List<string> ParseTags(string rawTagStr)
    {
        if (string.IsNullOrWhiteSpace(rawTagStr))
        {
            return null;
        }
        var tags = new List<string>(rawTagStr.Split(',').Select(t => t.Trim().ToLower()).Where(t => !string.IsNullOrEmpty(t)));
        return tags.Count > 0 ? tags : null;
    }

    //public const string SimpleContainerSep = ",;|";

    public static string GetBeanSep(TBean type)
    {
        if (type.Tags != null && type.Tags.TryGetValue("sep", out var s) && !string.IsNullOrWhiteSpace(s))
        {
            return s;
        }
        return ((DefBean)type.DefBean).Sep;
    }

    public static bool IsCollectionEqual(List<DType> a, List<DType> b)
    {
        if (a.Count == b.Count)
        {
            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (!object.Equals(a[i], b[i]))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public static string GetImplTypeName(DBean bean)
    {
        return GetImplTypeName(bean.ImplType, bean.Type);
    }

    public static string GetImplTypeName(DefBean implType, DefBean baseType)
    {
        if (implType.Namespace == baseType.Namespace)
        {
            return implType.Name;
        }
        else
        {
            return implType.FullName;
        }
    }

    public static DefBean GetImplTypeByNameOrAlias(DefBean bean, string subType)
    {
        if (string.IsNullOrEmpty(subType))
        {
            throw new Exception($"module:'{bean.Namespace}' 多态数据type不能为空");
        }
        DefBean defType = bean.GetHierarchyChildren().FirstOrDefault(c => c.Alias == subType || c.Name == subType || c.FullName == subType);
        if (defType == null)
        {
            throw new Exception($"module:'{bean.Namespace}' type:'{subType}' 不是合法类型");
        }
        if (defType.IsAbstractType)
        {
            throw new Exception($"module:'{bean.Namespace}' type:'{subType}' 是抽象类. 不能创建实例");
        }
        return defType;
    }

    private const string TAG_UNCHECKED = "unchecked";

    public static bool IsUnchecked(Record rec)
    {
        return rec.Tags != null && rec.Tags.Count > 0 && rec.Tags.Contains(TAG_UNCHECKED);
    }
}
