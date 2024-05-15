using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class DeserializeBinVisitor : DecoratorFuncVisitor<string, string, string, int, string>
{
    public static DeserializeBinVisitor Ins { get; } = new DeserializeBinVisitor();

    public override string DoAccept(TType type, string fieldName, string bufName, string err, int depth)
    {
        if (type.IsNullable)
        {
            return $"{{ var __exists__ bool; if __exists__, {err} = {bufName}.ReadBool(); {err} != nil {{ return }}; if __exists__ {{ var __x__ {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(BinUnderlyingDeserializeVisitor.Ins, "__x__", bufName, err, depth + 1)}; {fieldName} = {(type.Apply(IsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {
            return type.Apply(BinUnderlyingDeserializeVisitor.Ins, fieldName, bufName, err, depth);
        }
    }
}
