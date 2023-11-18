using Luban.Types;

namespace Luban.TypeVisitors;

class IsValidTableKeyTypeVisitor : AllTrueVisitor
{
    public static IsValidTableKeyTypeVisitor Ins { get; } = new();

    public override bool Accept(TDateTime type)
    {
        return false;
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
