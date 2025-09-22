using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Kotlin.TypeVisitors;

public class KotlinDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static KotlinDeclaringTypeNameVisitor Ins { get; } = new();

    public virtual string Accept(TBool type)
    {
        return type.IsNullable ? "Boolean?" : "Boolean";
    }

    public virtual string Accept(TByte type)
    {
        return type.IsNullable ? "Byte?" : "Byte";
    }

    public virtual string Accept(TShort type)
    {
        return type.IsNullable ? "Short?" : "Short";
    }

    public virtual string Accept(TInt type)
    {
        return type.IsNullable ? "Int?" : "Int";
    }

    public virtual string Accept(TLong type)
    {
        return type.IsNullable ? "Long?" : "Long";
    }

    public virtual string Accept(TFloat type)
    {
        return type.IsNullable ? "Float?" : "Float";
    }

    public virtual string Accept(TDouble type)
    {
        return type.IsNullable ? "Double?" : "Double";
    }

    public virtual string Accept(TEnum type)
    {
        string src = type.IsNullable ? "Int?" : "Int";
        return type.DefEnum.TypeNameWithTypeMapper() ?? src;
    }

    public string Accept(TString type)
    {
        return type.IsNullable ? "String?" : "String";
    }

    public virtual string Accept(TDateTime type)
    {
        return type.IsNullable ? "Long?" : "Long";
    }

    public string Accept(TBean type)
    {
        var typeName = type.DefBean.TypeNameWithTypeMapper() ?? type.DefBean.FullNameWithTopModule;
        return type.IsNullable ? $"{typeName}?" : typeName;
    }

    public string Accept(TArray type)
    {
        return $"Array<{type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TList type)
    {
        return $"MutableList<{type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TSet type)
    {
        return $"MutableSet<{type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TMap type)
    {
        return $"MutableMap<{type.KeyType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}, {type.ValueType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}>";
    }
}