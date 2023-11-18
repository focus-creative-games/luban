using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class IsPointerTypeVisitor : DecoratorFuncVisitor<bool>
{
    public static IsPointerTypeVisitor Ins { get; } = new();

    public override bool DoAccept(TType type)
    {
        return type.IsNullable;
    }

    public override bool Accept(TBean type)
    {
        return false;
    }

    public override bool Accept(TArray type)
    {
        return false;
    }

    public override bool Accept(TList type)
    {
        return false;
    }

    public override bool Accept(TSet type)
    {
        return false;
    }

    public override bool Accept(TMap type)
    {
        return false;
    }
}
