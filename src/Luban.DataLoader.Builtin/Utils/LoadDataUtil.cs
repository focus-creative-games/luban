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

using System.Text;
using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader.Builtin.Utils;

static class LoadDataUtil
{
    public static string[] SplitVectorString(string x)
    {
        return x.Split(',', '_', ';');
    }

    public static string[] SplitStringByAnySepChar(string x, string sep)
    {
        return x.Split(sep.ToCharArray());
    }

    // public static DType CreateVector(TVector2 type, string x)
    // {
    //     var values = SplitVectorString(x);
    //     if (values.Length != 2)
    //     {
    //         throw new Exception($"'{x}' 不是合法vector2类型数据");
    //     }
    //     return new DVector2(new System.Numerics.Vector2(float.Parse(values[0]), float.Parse(values[1])));
    //
    // }
    //
    // public static DType CreateVector(TVector3 type, string x)
    // {
    //     var values = SplitVectorString(x);
    //     if (values.Length != 3)
    //     {
    //         throw new Exception($"'{x}' 不是合法vector3类型数据");
    //     }
    //     return new DVector3(new System.Numerics.Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));
    //
    // }
    //
    // public static DType CreateVector(TVector4 type, string x)
    // {
    //     var values = SplitVectorString(x);
    //     if (values.Length != 4)
    //     {
    //         throw new Exception($"'{x}' 不是合法vector4类型数据");
    //     }
    //     return new DVector4(new System.Numerics.Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
    // }

    //public static DDateTime CreateDateTime(string x, TimeZoneInfo timeZoneInfo)
    //{

    //    DateTime dateTime = DateTime.ParseExact(x,
    //        new string[] {
    //            "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH", "yyyy-MM-dd",
    //            //"yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH", "yyyy/MM/dd",
    //        },
    //        System.Globalization.CultureInfo.InvariantCulture);
    //    return new DDateTime(TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo));
    //}
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

    public static (string Key, string Text) ExtractText(string rawKeyAndText)
    {
        string[] keyAndText = rawKeyAndText.Split('|');
        if (keyAndText.Length != 2)
        {
            throw new Exception("text data should like <key>|<text>");
        }
        return (keyAndText[0], keyAndText[1]);
    }

    public static void ValidateText(string key, string text)
    {
        if (key == null || text == null)
        {
            throw new Exception("text的key或text属性不能为null");
        }
        if (key == "" && text != "")
        {
            throw new Exception($"text  key为空, 但text:'{text}'不为空");
        }
    }


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


    private const string TAG_UNCHECKED = "unchecked";

    public static bool IsUnchecked(Record rec)
    {
        return rec.Tags != null && rec.Tags.Count > 0 && rec.Tags.Contains(TAG_UNCHECKED);
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
        DefBean defType = bean.GetHierarchyChildren().Cast<DefBean>().Where(c => c.Alias == subType || c.Name == subType || c.FullName == subType).FirstOrDefault();
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

    public static bool ParseExcelBool(string s)
    {
        s = s.ToLower().Trim();
        switch (s)
        {
            case "true":
            case "1":
            case "y":
            case "yes":
                return true;
            case "false":
            case "0":
            case "n":
            case "no":
                return false;
            default:
                throw new InvalidExcelDataException($"{s} 不是 bool 类型的值 (true|1|y|yes 或 false|0|n|no)");
        }
    }

    public static bool TryParseExcelByteFromNumberOrConstAlias(string s, out byte value)
    {
        s = s.Trim();
        if (byte.TryParse(s, out value))
        {
            return true;
        }
        DefAssembly assembly = GenerationContext.Current.Assembly;
        if (assembly.TryGetConstAlias(s, out var constValue) && byte.TryParse(constValue, out value))
        {
            return true;
        }
        return false;
    }

    public static bool TryParseExcelShortFromNumberOrConstAlias(string s, out short value)
    {
        s = s.Trim();
        if (short.TryParse(s, out value))
        {
            return true;
        }
        DefAssembly assembly = GenerationContext.Current.Assembly;
        if (assembly.TryGetConstAlias(s, out var constValue) && short.TryParse(constValue, out value))
        {
            return true;
        }
        return false;
    }

    public static bool TryParseExcelIntFromNumberOrConstAlias(string s, out int value)
    {
        s = s.Trim();
        if (s.StartsWith("0x") || s.StartsWith("0X"))
        {
            return int.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out value);
        }
        if (int.TryParse(s, out value))
        {
            return true;
        }
        DefAssembly assembly = GenerationContext.Current.Assembly;
        if (assembly.TryGetConstAlias(s, out var constValue) && int.TryParse(constValue, out value))
        {
            return true;
        }
        return false;
    }

    public static bool TryParseExcelLongFromNumberOrConstAlias(string s, out long value)
    {
        s = s.Trim();
        if (long.TryParse(s, out value))
        {
            return true;
        }
        DefAssembly assembly = GenerationContext.Current.Assembly;
        if (assembly.TryGetConstAlias(s, out var constValue) && long.TryParse(constValue, out value))
        {
            return true;
        }
        return false;
    }

    public static bool TryParseExcelFloatFromNumberOrConstAlias(string s, out float value)
    {
        s = s.Trim();
        if (float.TryParse(s, out value))
        {
            return true;
        }
        DefAssembly assembly = GenerationContext.Current.Assembly;
        if (assembly.TryGetConstAlias(s, out var constValue) && float.TryParse(constValue, out value))
        {
            return true;
        }
        return false;
    }

    public static bool TryParseExcelDoubleFromNumberOrConstAlias(string s, out double value)
    {
        s = s.Trim();
        if (double.TryParse(s, out value))
        {
            return true;
        }
        DefAssembly assembly = GenerationContext.Current.Assembly;
        if (assembly.TryGetConstAlias(s, out var constValue) && double.TryParse(constValue, out value))
        {
            return true;
        }
        return false;
    }

    //public static string Data2String(DType data)
    //{
    //    var s = new StringBuilder();
    //    data.Apply(VisitorToString.Ins, s);
    //    return s.ToString();
    //}
}
