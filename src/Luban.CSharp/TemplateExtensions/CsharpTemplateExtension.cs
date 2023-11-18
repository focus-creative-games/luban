using Luban.CodeFormat;
using Luban.CSharp.TypeVisitors;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class CsharpTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(DeclaringTypeNameVisitor.Ins);
    }

    public static string DeclaringCollectionRefName(TType type)
    {
        return type.Apply(DeclaringCollectionRefNameVisitor.Ins);
    }

    public static string ClassOrStruct(DefBean bean)
    {
        return bean.IsValueType ? "struct" : "class";
    }

    public static string ClassModifier(DefBean bean)
    {
        return bean.IsAbstractType ? "abstract" : "sealed";
    }

    public static string MethodModifier(DefBean bean)
    {
        return bean.ParentDefType != null ? "override" : (bean.IsAbstractType ? "virtual" : "");
    }

    public static string NamespaceWithGraceBegin(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return string.Empty;
        }
        return $"namespace {ns}\n{{";
    }

    public static string NamespaceWithGraceEnd(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return string.Empty;
        }
        return "}";
    }

    public static string ToPrettyString(string name, TType type)
    {
        return type.Apply(DataToStringVisitor.Ins, name);
    }

    public static string GetValueOfNullableType(TType type, string varName)
    {
        return type.Apply(IsRawNullableTypeVisitor.Ins) ? varName : $"{varName}.Value";
    }

    // public static string RefTypeName(DefField field)
    // {
    //     if (field.CType.GetTag("ref") is { } value && GenerationContext.Current.Assembly.GetCfgTable(value) is { } cfgTable)
    //     {
    //         return cfgTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins);
    //     }
    //     return string.Empty;
    // }
}
