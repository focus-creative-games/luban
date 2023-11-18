using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class DeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static DeclaringTypeNameVisitor Ins { get; } = new DeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        return type.IsNullable ? $"{type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}|undefined" : type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins);
    }
}
