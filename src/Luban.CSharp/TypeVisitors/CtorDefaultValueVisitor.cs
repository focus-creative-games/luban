using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class CtorDefaultValueVisitor : DecoratorFuncVisitor<string>
{
    public static CtorDefaultValueVisitor Ins { get; } = new();

    public override string DoAccept(TType type)
    {
        return "default";
    }

    public override string Accept(TString type)
    {
        return "\"\"";
    }

    public override string Accept(TBean type)
    {
        return type.DefBean.IsAbstractType ? "default" : $"new {type.Apply(DeclaringTypeNameVisitor.Ins)}()";
    }

    public override string Accept(TArray type)
    {
        return $"System.Array.Empty<{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TList type)
    {
        return $"new {ConstStrings.ListTypeName}<{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TSet type)
    {
        return $"new {ConstStrings.HashSetTypeName}<{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TMap type)
    {
        return $"new {ConstStrings.HashMapTypeName}<{type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)},{type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)}>()";
    }
}
