using System.Text;
using Luban.Cpp.TypeVisitors;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Cpp.TemplateExtensions;

public class CppTemplateExtension : ScriptObject
{
    public static string MakeTypeCppName(DefTypeBase type)
    {
        return TypeUtil.MakeCppFullName(type.Namespace, type.Name);
    }

    public static string MakeCppName(string typeName)
    {
        return TypeUtil.MakeCppFullName("", typeName);
    }

    public static string GetterName(string originName)
    {
        var words = originName.Split('_').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        var s = new StringBuilder("get");
        foreach (var word in words)
        {
            s.Append(TypeUtil.UpperCaseFirstChar(word));
        }
        return s.ToString();
    }

    public static string NamespaceWithGraceBegin(string ns)
    {
        return TypeUtil.MakeCppNamespaceBegin(ns);
    }

    public static string NamespaceWithGraceEnd(string ns)
    {
        return TypeUtil.MakeCppNamespaceEnd(ns);
    }

    public static string GetValueOfNullableType(TType type, string varName)
    {
        return $"(*({varName}))";
    }

}
