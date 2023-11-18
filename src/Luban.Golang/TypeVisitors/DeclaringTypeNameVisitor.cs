using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class DeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static DeclaringTypeNameVisitor Ins { get; } = new DeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        var s = type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins);
        return type.Apply(IsPointerTypeVisitor.Ins) ? "*" + s : s;
    }
}
