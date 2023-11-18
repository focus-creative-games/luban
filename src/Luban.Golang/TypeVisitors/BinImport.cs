using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class BinImport : DecoratorActionVisitor<HashSet<string>>
{
    public static BinImport Ins { get; } = new();

    public override void DoAccept(TType type, HashSet<string> x)
    {

    }

    public override void Accept(TArray type, HashSet<string> x)
    {
        type.ElementType.Apply(this, x);
    }

    public override void Accept(TList type, HashSet<string> x)
    {
        type.ElementType.Apply(this, x);
    }

    public override void Accept(TSet type, HashSet<string> x)
    {

    }

    public override void Accept(TMap type, HashSet<string> x)
    {
        type.KeyType.Apply(this, x);
        type.ValueType.Apply(this, x);
    }
}
