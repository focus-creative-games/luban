using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.FlatBuffers.TypeVisitors;

public class FlatBuffersTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static FlatBuffersTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "ubyte";
    }

    public string Accept(TShort type)
    {
        return "int16";
    }

    public string Accept(TInt type)
    {
        return "int32";
    }

    public string Accept(TLong type)
    {
        return "int64";
    }

    public string Accept(TFloat type)
    {
        return "float32";
    }

    public string Accept(TDouble type)
    {
        return "float64";
    }

    public string Accept(TEnum type)
    {
        return TypeUtil.MakeFlatBuffersFullName(type.DefEnum.Namespace, type.DefEnum.Name);
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public string Accept(TDateTime type)
    {
        return "int64";
    }

    public string Accept(TBean type)
    {
        return TypeUtil.MakeFlatBuffersFullName(type.DefBean.Namespace, type.DefBean.Name);
    }

    public string Accept(TArray type)
    {
        return $"[{type.ElementType.Apply(this)}]";
    }

    public string Accept(TList type)
    {
        return $"[{type.ElementType.Apply(this)}]";
    }

    public string Accept(TSet type)
    {
        return $"[{type.ElementType.Apply(this)}]";
    }

    public string Accept(TMap type)
    {
        return $"[KeyValue_{type.KeyType.Apply(this)}_{type.ValueType.Apply(this)}]";
    }
}
