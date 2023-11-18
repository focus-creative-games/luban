using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

class JsonImport : DecoratorActionVisitor<HashSet<string>>
{
    public static JsonImport Ins { get; } = new();

    public override void DoAccept(TType type, HashSet<string> x)
    {
        x.Add("errors");
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
        type.ElementType.Apply(this, x);
    }

    public override void Accept(TMap type, HashSet<string> x)
    {
        type.KeyType.Apply(this, x);
        type.ValueType.Apply(this, x);
    }
}
