using Luban.Defs;
using Luban.Java.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Java.TemplateExtensions;

public class JavaCommonTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(JavaDeclaringTypeNameVisitor.Ins);
    }

    public static string DeclaringBoxTypeName(TType type)
    {
        return type.Apply(JavaDeclaringBoxTypeNameVisitor.Ins);
    }

    public static string ClassModifier(DefBean type)
    {
        return type.IsAbstractType ? "abstract" : "final";
    }

    public static string GetterName(string name)
    {
        return TypeUtil.ToJavaGetterName(name);
    }
}
