using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Kotlin.TypeVisitors;

public class KotlinDeclaringBoxTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static KotlinDeclaringBoxTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "Boolean";
    }

    public string Accept(TByte type)
    {
        return "Byte";
    }

    public string Accept(TShort type)
    {
        return "Short";
    }

    public string Accept(TInt type)
    {
        return "Int";
    }

    public string Accept(TLong type)
    {
        return "Long";
    }

    public string Accept(TFloat type)
    {
        return "Float";
    }

    public string Accept(TDouble type)
    {
        return "Double";
    }

    public string Accept(TEnum type)
    {
        return type.DefEnum.TypeNameWithTypeMapper() ?? "Int";
    }

    public string Accept(TString type)
    {
        return "String";
    }

    public string Accept(TDateTime type)
    {
        return "Long";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.TypeNameWithTypeMapper() ?? type.DefBean.FullNameWithTopModule;
    }

    public string Accept(TArray type)
    {
        return $"Array<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TList type)
    {
        return $"MutableList<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"MutableSet<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"MutableMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }
}