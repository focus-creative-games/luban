using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class UnderlyingDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static UnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "boolean";
    }

    public string Accept(TByte type)
    {
        return "number";
    }

    public string Accept(TShort type)
    {
        return "number";
    }

    public string Accept(TInt type)
    {
        return "number";
    }

    public string Accept(TLong type)
    {
        return type.IsBigInt ? "bigint" : "number";
    }

    public string Accept(TFloat type)
    {
        return "number";
    }

    public string Accept(TDouble type)
    {
        return "number";
    }

    public string Accept(TEnum type)
    {
        return type.DefEnum.FullName;
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.FullName;
    }

    public virtual string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public virtual string Accept(TList type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public virtual string Accept(TSet type)
    {
        return $"Set<{type.ElementType.Apply(this)}>";
    }

    public virtual string Accept(TMap type)
    {
        return $"Map<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }

    public string Accept(TDateTime type)
    {
        return "number";
    }
}
