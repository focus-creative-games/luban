using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class GoDeserializeBinVisitor : DecoratorFuncVisitor<string, string, string, string>
{
    public static GoDeserializeBinVisitor Ins { get; } = new GoDeserializeBinVisitor();

    public override string DoAccept(TType type, string fieldName, string bufName, string err)
    {
        if (type.IsNullable)
        {
            return $"{{ var __exists__ bool; if __exists__, {err} = {bufName}.ReadBool(); {err} != nil {{ return }}; if __exists__ {{ var __x__ {type.Apply(GoUnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(GoBinUnderlyingDeserializeVisitor.Ins, "__x__", bufName, err)}; {fieldName} = {(type.Apply(GoIsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {
            return type.Apply(GoBinUnderlyingDeserializeVisitor.Ins, fieldName, bufName, err);
        }
    }
}