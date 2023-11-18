
using Luban.CodeFormat;
using Luban.Defs;
using Luban.Protobuf.DataTarget;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Protobuf.TemplateExtensions;

public class ProtobufCommonTemplateExtension : ScriptObject
{

    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakePbFullName(type.Namespace, type.Name);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(ProtobufTypeNameVisitor.Ins);
    }

    public static string SuffixOptions(TType type)
    {
        if (type.IsCollection && !(type is TMap))
        {
            return $"[packed = {(type.ElementType.Apply(IsProtobufPackedType.Ins) ? "true" : "false")}]";
        }
        else
        {
            return "";
        }
    }
}
