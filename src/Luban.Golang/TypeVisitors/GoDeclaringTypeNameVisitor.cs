using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class GoDeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static GoDeclaringTypeNameVisitor Ins { get; } = new GoDeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        var s = type.Apply(GoUnderlyingDeclaringTypeNameVisitor.Ins);
        return type.Apply(GoIsPointerTypeVisitor.Ins) ? "*" + s : s;
    }
}