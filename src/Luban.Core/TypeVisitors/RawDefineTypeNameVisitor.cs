using Luban.Types;

namespace Luban.TypeVisitors;

public class RawDefineTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static RawDefineTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "byte";
    }

    public string Accept(TShort type)
    {
        return "short";
    }

    public string Accept(TInt type)
    {
        return "int";
    }

    public string Accept(TLong type)
    {
        return "long";
    }

    public string Accept(TFloat type)
    {
        return "float";
    }

    public string Accept(TDouble type)
    {
        return "double";
    }

    public string Accept(TEnum type)
    {
        return type.DefEnum.FullName;
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public virtual string Accept(TDateTime type)
    {
        return "datetime";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.FullName;
    }

    public string Accept(TArray type)
    {
        return $"array,{type.ElementType.Apply(this)}";
    }

    public string Accept(TList type)
    {
        return $"list,{type.ElementType.Apply(this)}";
    }

    public string Accept(TSet type)
    {
        return $"set,{type.ElementType.Apply(this)}";
    }

    public string Accept(TMap type)
    {
        return $"map,{type.KeyType.Apply(this)},{type.ValueType.Apply(this)}";
    }
}
