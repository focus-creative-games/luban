using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class TypescriptDefineTypeNameUnderlyingVisitor : ITypeFuncVisitor<string>
{
    public static TypescriptDefineTypeNameUnderlyingVisitor Ins { get; } = new();

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


    private string GetArrayType(TType elementType)
    {
        switch (elementType)
        {
            case TByte _: return "Uint8Array";
            case TShort _:
            case TInt _:
            case TLong _:
            case TFloat _: return "Float32Array";
            case TDouble _: return "Float64Array";
            default: return $"{elementType.Apply(this)}[]";
        }
    }

    public virtual string Accept(TArray type)
    {
        return GetArrayType(type.ElementType);
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
