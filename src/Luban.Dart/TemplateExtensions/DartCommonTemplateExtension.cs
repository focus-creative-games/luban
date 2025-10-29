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

﻿using Luban.Dart.TypeVisitors;
using Luban.Defs;
using Luban.TemplateExtensions;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Dart.TemplateExtensions;

class DartCommonTemplateExtension : ScriptObject
{
    public static string ClassModifier(DefBean type)
    {
        return type.IsAbstractType ? "abstract" : "";
    }
    public static string MethodModifier(DefBean bean)
    {
        return bean.ParentDefType != null ? "@override" : (bean.IsAbstractType ? "" : "");
    }
    public static bool IsLastEnumItem(DefEnum defEnum, int value)
    {
        return defEnum.Items.Last().IntValue == value;
    }
    public static bool HasEnumItem(DefEnum defEnum)
    {
        return defEnum.Items.Count > 0;
    }
    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(DartDeclaringTypeNameVisitor.Ins);
    }
    public static string DeclaringNullTypeName(TType type)
    {
        var name = type.Apply(DartDeclaringTypeNameVisitor.Ins);
        if (name.EndsWith('?'))
        {
            return name;
        }
        return name + "?";//dart get must be nullable
    }
    public static string DeclaringCollectionRefName(TType type)
    {
        return type.Apply(DeclaringCollectionRefNameVisitor.Ins);
    }
    public static string DeclaringImportBean(DefBean bean, string RootFolder)
    {
        return $"import '{RootFolder}{GetImportName(bean.FullName)}.dart';\n";
    }
    public static string DeclaringImportTable(DefTable table, string RootFolder)
    {
        return $"import '{RootFolder}{GetImportName(table.FullName)}.dart';\n";
    }
    //根据类型导入对应文件
    public static string DeclaringImportName(TType type, string RootFolder)
    {
        if (type is TEnum @enum)
        {
            return $"import '{RootFolder}{GetImportName(@enum.DefEnum.FullName)}.dart';\n";
        }
        else if (type is TBean bean)
        {
            return $"import '{RootFolder}{GetImportName(bean.DefBean.FullName)}.dart';\n";
        }
        else if (type.IsCollection)
        {
            var tType = type.ElementType;

            if (type is TMap map)
            {
                return DeclaringImportName(map.KeyType, RootFolder) + DeclaringImportName(map.ValueType, RootFolder);
            }
            return DeclaringImportName(tType, RootFolder);
        }

        return "";
    }
    public static bool NeedGenConstructor(List<DefField> fields)
    {
        return fields.Count > 0;
    }
    public static bool IsSuperField(DefField field, DefBean parent)
    {
        if (parent == null)
        {
            return false;
        }
        if (parent.ExportFields.Contains(field))
        {
            return true;
        }
        if (parent.ParentDefType != null)
        {
            return IsSuperField(field, parent.ParentDefType);
        }
        return false;
    }
    public static string GetImportName(string name)
    {
        return name.Replace('.', '/');
    }
    public static string FormatNameLowerCamel(string s)
    {
        return char.ToLower(s[0]) + s.Substring(1);
    }
    public static string GetValueOfNullableType(TType type, string varName)
    {
        return varName + "!";
    }
    public static string ImportRefTypeDart(DefField field, string RootFolder)
    {
        if (TypeTemplateExtension.CanGenerateRef(field))
        {
            var ref_table = TypeTemplateExtension.GetRefTable(field);
            if (ref_table != null)
            {
                return DeclaringImportBean(ref_table.ValueTType.DefBean, RootFolder);
            }
        }
        if (TypeTemplateExtension.CanGenerateCollectionRef(field))
        {
            var ref_table = TypeTemplateExtension.GetCollectionRefTable(field);
            if (ref_table != null)
            {
                return DeclaringImportBean(ref_table.ValueTType.DefBean, RootFolder);
            }
        }
        return DeclaringImportName(field.CType, RootFolder);
    }
}
