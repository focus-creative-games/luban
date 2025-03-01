using Luban.Dart.TypeVisitors;
using Luban.Defs;
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
        if (type is TBean bean)
        {
            return $"import '{RootFolder}{GetImportName(bean.DefBean.FullName)}.dart';\n";
        }
        if (type.IsCollection)
        {
            var tType = type.ElementType;

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
    public static string ImportRefTypeDart(DefField field, string RootFolder)
    {
        return DeclaringImportName(field.CType, RootFolder);
    }
}
