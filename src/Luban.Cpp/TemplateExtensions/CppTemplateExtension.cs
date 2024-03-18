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

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(DeclaringTypeNameVisitor.Ins);
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

    public static string GetBeansIncludes(List<DefField> fields)
    {
        var includes = new HashSet<string>();
        foreach (var field in fields)
        {
            if (field.CType.IsBean == false)
                continue;
                
            if (includes.Contains(field.Type))
                continue;

            includes.Add(field.Type);
        }
        return string.Join("\n", includes.Select(item => $"#include \"{TypeUtil.ToSnakeCase(item)}.h\""));
    }


    public static string ToSnakeCase(string str)
    {
        return TypeUtil.ToSnakeCase(str);
    }

    public static int GetIdByFullName(string fullName)
    {
        return TypeUtil.ComputeCfgHashIdByName(fullName);
    }
}
