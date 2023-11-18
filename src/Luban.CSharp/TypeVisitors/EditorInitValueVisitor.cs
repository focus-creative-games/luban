using Luban.Types;

namespace Luban.CSharp.TypeVisitors;

public class EditorInitValueVisitor : CtorDefaultValueVisitor
{
    public new static EditorInitValueVisitor Ins { get; } = new();

    public override string Accept(TEnum type)
    {
        return $"{(type.DefEnum.Items.Count > 0 ? $"{type.Apply(EditorDeclaringTypeNameVisitor.Ins)}." + type.DefEnum.Items[0].Name : "default")}";
    }

    public override string Accept(TDateTime type)
    {
        return "\"1970-01-01 00:00:00\"";
    }

    public override string Accept(TBean type)
    {
        return type.IsNullable || type.DefBean.IsAbstractType ? "default" : $"new {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}()";
    }

    public override string Accept(TArray type)
    {
        return $"System.Array.Empty<{type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TList type)
    {
        return $"new {ConstStrings.ListTypeName}<{type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TSet type)
    {
        return $"new {ConstStrings.HashSetTypeName}<{type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TMap type)
    {
        return $"new {ConstStrings.HashMapTypeName}<{type.KeyType.Apply(EditorDeclaringTypeNameVisitor.Ins)},{type.ValueType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }
}
