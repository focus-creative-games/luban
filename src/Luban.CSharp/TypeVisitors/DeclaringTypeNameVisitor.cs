using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class DeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static DeclaringTypeNameVisitor Ins { get; } = new();

    protected virtual ITypeFuncVisitor<string> UnderlyingVisitor => UnderlyingDeclaringTypeNameVisitor.Ins;

    public override string DoAccept(TType type)
    {
        return type.IsNullable && !type.Apply(IsRawNullableTypeVisitor.Ins) ? (type.Apply(UnderlyingVisitor) + "?") : type.Apply(UnderlyingVisitor);
    }

    public override string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public override string Accept(TList type)
    {
        return $"{ConstStrings.ListTypeName}<{type.ElementType.Apply(this)}>";
    }

    public override string Accept(TSet type)
    {
        return $"{ConstStrings.HashSetTypeName}<{type.ElementType.Apply(this)}>";
    }

    public override string Accept(TMap type)
    {
        return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }
}
