using Luban.Defs;
using Luban.Gdscript.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Gdscript.TemplateExtensions;

public class GdscriptCommonTemplateExtension : ScriptObject
{

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(DeclaringTypeNameVisitor.Ins);
    }

    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakeGDScriptFullName(type.Namespace, type.Name);
    }

    public static string StrFullName(string fullName)
    {
        return TypeUtil.ToPascalCase(fullName.Replace(".", "_"));
    }
}
