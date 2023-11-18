using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class DeserializeJson2Visitor : DecoratorFuncVisitor<string, string, string>
{
    public static DeserializeJson2Visitor Ins { get; } = new();

    public override string DoAccept(TType type, string varName, string bufName)
    {
        if (type.IsNullable)
        {
            return $"{{ if {bufName} == nil {{ return }} else {{ var __x__ {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(DeserializeJsonUndering2Visitor.Ins, "__x__", bufName)}; {varName} = {(type.Apply(IsPointerTypeVisitor.Ins) ? "&" : "")}__x__ }}}}";
        }
        else
        {
            return type.Apply(DeserializeJsonUndering2Visitor.Ins, varName, bufName);
        }
    }
}
