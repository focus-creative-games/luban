using Luban.Defs;
using Luban.Kotlin.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Kotlin.TemplateExtensions;

public class KotlinCommonTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(KotlinDeclaringTypeNameVisitor.Ins);
    }

    public static string DeclaringBoxTypeName(TType type)
    {
        return type.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins);
    }

    public static string ClassModifier(DefBean type)
    {
        return type.IsAbstractType ? "abstract" : "";
    }

    public static string GetterName(string name)
    {
        return name; // Kotlin uses property syntax, no need for getter prefix
    }

    public static string PropertyModifier(DefField field)
    {
        return "val"; // In Kotlin, use 'val' for immutable properties
    }
}