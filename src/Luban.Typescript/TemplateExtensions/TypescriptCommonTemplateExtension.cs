using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Luban.Typescript.TypeVisitors;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Typescript.TemplateExtensions;

public class TypescriptCommonTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(DeclaringTypeNameVisitor.Ins);
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
            return "";
        }
        return string.Join("", ns.Split('.').Select(n => $"export namespace {n} {{"));
    }

    public static string NamespaceWithGraceEnd(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return "";
        }
        return string.Join("", ns.Split('.').Select(n => $"}}"));
    }

}
