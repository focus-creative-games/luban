using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class DeserializeJsonVisitor : DecoratorFuncVisitor<string, string, int, string>
{
    public static DeserializeJsonVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string varName, string bufName, int depth)
    {
        if (type.IsNullable)
        {
            return $"{{ if {bufName} == nil {{ return }} else {{ var __x{depth}__ {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)};  {type.Apply(DeserializeJsonUnderingVisitor.Ins, $"__x{depth}__", bufName, depth)}; {varName} = {(type.Apply(IsPointerTypeVisitor.Ins) ? "&" : "")}__x{depth}__ }}}}";
        }
        else
        {
            return type.Apply(DeserializeJsonUnderingVisitor.Ins, varName, bufName, depth);
        }
    }
}
