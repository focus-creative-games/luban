namespace Luban.Any.TypeVisitors;

public class IsCollectionType : AllFalseVisitor
{
    public static IsCollectionType Ins { get; } = new();


    public override bool Accept(TArray type)
    {
        return true;
    }

    public override bool Accept(TList type)
    {
        return true;
    }

    public override bool Accept(TSet type)
    {
        return true;
    }

    public override bool Accept(TMap type)
    {
        return true;
    }
}