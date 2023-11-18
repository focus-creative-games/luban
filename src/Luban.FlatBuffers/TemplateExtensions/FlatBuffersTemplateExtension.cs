using Luban.Defs;
using Luban.FlatBuffers.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.FlatBuffers.TemplateExtensions;

public class FlatBuffersTemplateExtension : ScriptObject
{
    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakeFlatBuffersFullName(type.Namespace, type.Name);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(FlatBuffersTypeNameVisitor.Ins);
    }

    public static string TypeMetadata(TType type)
    {
        return type.IsNullable || type.Apply(IsFlatBuffersScalarTypeVisitor.Ins) ? "" : "(required)";
    }
}
