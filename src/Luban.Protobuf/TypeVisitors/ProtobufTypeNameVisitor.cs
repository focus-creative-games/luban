using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Protobuf.TypeVisitors;

public class ProtobufTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static ProtobufTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "int32";
    }

    public string Accept(TShort type)
    {
        return "int32";
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
        return "float";
    }

    public string Accept(TDouble type)
    {
        return "double";
    }

    public string Accept(TEnum type)
    {
        return TypeUtil.MakePbFullName(type.DefEnum.Namespace, type.DefEnum.Name);
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
        return TypeUtil.MakePbFullName(type.DefBean.Namespace, type.DefBean.Name);
    }

    public string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}";
    }

    public string Accept(TList type)
    {
        return $"{type.ElementType.Apply(this)}";
    }

    public string Accept(TSet type)
    {
        return $"{type.ElementType.Apply(this)}";
    }

    public string Accept(TMap type)
    {
        string key = type.KeyType is TEnum ? "int32" : (type.KeyType.Apply(this));
        return $"map<{key}, {type.ValueType.Apply(this)}>";
    }
}
