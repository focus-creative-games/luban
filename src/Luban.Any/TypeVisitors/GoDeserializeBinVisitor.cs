namespace Luban.Any.TypeVisitors;

public class GoDeserializeBinVisitor : DecoratorFuncVisitor<string, string, string, string>
{
    public static GoDeserializeBinVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string fieldName, string bufName, string err)
    {
        if (type.IsNullable)
        {
            return $"{{ var __exists__ bool; if __exists__, {err} = {bufName}.ReadBool(); {err} != nil {{ return }}; if __exists__ {{ var __x__ {type.Apply(GoTypeUnderingNameVisitor.Ins)};  {type.Apply(GoDeserializeUnderingVisitor.Ins, "__x__", bufName, err)}; {fieldName} = {(type.Apply(GoIsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {
            return type.Apply(GoDeserializeUnderingVisitor.Ins, fieldName, bufName, err);
        }
    }
}