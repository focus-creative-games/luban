using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.PHP.TemplateExtensions;

public class PHPCommonTemplateExtension : ScriptObject
{
    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakePyFullName(type.Namespace, type.Name);
    }

    public static string ClassModifier(DefBean bean)
    {
        return bean.IsAbstractType ? "abstract" : "";
    }

    public static string NamespaceWithGraceBegin(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return "";
        }
        return string.Join("", ns.Split('.').Select(n => $"namespace {n} {{"));
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
