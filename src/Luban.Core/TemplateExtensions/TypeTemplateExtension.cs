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

using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class TypeTemplateExtension : ScriptObject
{
    public static bool NeedMarshalBoolPrefix(TType type)
    {
        return type.IsNullable;
    }

    public static string FormatMethodName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatMethod(name);
    }

    public static string FormatFieldName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatField(name);
    }

    public static string FormatPropertyName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatProperty(name);
    }

    public static string FormatEnumItemName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatEnumItemName(name);
    }

    public static bool CanGenerateRef(DefField field)
    {
        if (field.CType.IsCollection)
        {
            return false;
        }

        return GetRefTable(field) != null;
    }

    public static bool CanGenerateCollectionRef(DefField field)
    {
        if (!field.CType.IsCollection)
        {
            return false;
        }
        return GetCollectionRefTable(field) != null;
    }

    public static DefTable GetCollectionRefTable(DefField field)
    {
        var refTag = field.CType.GetTag("ref");
        if (refTag == null)
        {
            refTag = field.CType.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return null;
        }
        if (GenerationContext.Current.Assembly.GetCfgTable(refTag.Replace("?", "")) is { } cfgTable)
        {
            return cfgTable;
        }
        return null;
    }

    public static DefTable GetRefTable(DefField field)
    {
        if (field.CType.GetTag("ref") is { } value && GenerationContext.Current.Assembly.GetCfgTable(value.Replace("?", "")) is { } cfgTable)
        {
            return cfgTable;
        }
        return null;
    }

    public static TType GetRefType(DefField field)
    {
        return GetRefTable(field)?.ValueTType;
    }

    public static bool IsFieldBeanNeedResolveRef(DefField field)
    {
        return field.CType is TBean bean && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool IsBeanNeedResolveRef(TBean bean)
    {
        return bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool IsFieldArrayLikeNeedResolveRef(DefField field)
    {
        return field.CType.ElementType is TBean bean && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType && field.CType is not TMap;
    }

    public static bool IsFieldMapNeedResolveRef(DefField field)
    {
        return field.CType is TMap { ValueType: TBean bean } && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool HasIndex(DefField field)
    {
        TType type = field.CType;
        return type.HasTag("index") && type is TArray or TList;
    }

    public static string GetIndexName(DefField field)
    {
        return field.CType.GetTag("index");
    }

    public static DefField GetIndexField(DefField field)
    {
        string indexName = GetIndexName(field);
        return ((TBean)field.CType.ElementType).DefBean.GetField(indexName);
    }

    public static TMap GetIndexMapType(DefField field)
    {
        DefField indexField = GetIndexField(field);
        return TMap.Create(false, null, indexField.CType, field.CType.ElementType, false);
    }

    public static string ImplDataType(DefBean type, DefBean parent)
    {
        return DataUtil.GetImplTypeName(type, parent);
    }

    public static string EscapeComment(string comment)
    {
        return System.Web.HttpUtility.HtmlEncode(comment).Replace("\n", "<br/>");
    }
}
