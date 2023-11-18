using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class UnderlyingDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static UnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

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

    public virtual string Accept(TEnum type)
    {
        return type.DefEnum.TypeNameWithTypeMapper() ?? type.DefEnum.FullName;
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.TypeNameWithTypeMapper() ?? type.DefBean.FullName;
    }

    public string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public string Accept(TList type)
    {
        return $"{ConstStrings.ListTypeName}<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"{ConstStrings.HashSetTypeName}<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }

    public virtual string Accept(TDateTime type)
    {
        return "long";
    }
}
