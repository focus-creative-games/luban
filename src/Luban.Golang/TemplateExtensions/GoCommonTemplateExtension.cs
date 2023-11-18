using Luban.Defs;
using Luban.Golang.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Golang.TemplateExtensions;

public class GoCommonTemplateExtension : ScriptObject
{
    public static string FullName(DefTypeBase bean)
    {
        return TypeUtil.MakeGoFullName(bean.Namespace, bean.Name);
    }
    public static string FullNameLowerCase(DefTypeBase bean)
    {
        return TypeUtil.MakeGoFullName(bean.Namespace, bean.Name).ToLowerInvariant();
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(DeclaringTypeNameVisitor.Ins);
    }

}
