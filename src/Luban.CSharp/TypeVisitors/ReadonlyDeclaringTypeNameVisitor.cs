using Luban.Types;

namespace Luban.CSharp.TypeVisitors;

public class ReadonlyDeclaringTypeNameVisitor : DeclaringTypeNameVisitor
{
    public new static ReadonlyDeclaringTypeNameVisitor Ins { get; } = new();

    public override string Accept(TList type)
    {
        return $"IReadOnlyList<{type.ElementType.Apply(this)}>";
    }

    public override string Accept(TSet type)
    {
        return $"IReadOnlyHashSet<{type.ElementType.Apply(this)}>";
    }

    public override string Accept(TMap type)
    {
        return $"IReadOnlyDictionary<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }
}