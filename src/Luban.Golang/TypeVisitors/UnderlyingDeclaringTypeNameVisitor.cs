using Luban.Golang.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Golang.TypeVisitors;

public class UnderlyingDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static UnderlyingDeclaringTypeNameVisitor Ins { get; } = new UnderlyingDeclaringTypeNameVisitor();

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
        return "int32";
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.IsAbstractType ? $"interface{{}}" : $"*{GoCommonTemplateExtension.FullName(type.DefBean)}";
    }

    public string Accept(TArray type)
    {
        return $"[]{type.ElementType.Apply(this)}";
    }

    public string Accept(TList type)
    {
        return $"[]{type.ElementType.Apply(this)}";
    }

    public string Accept(TSet type)
    {
        return $"[]{type.ElementType.Apply(this)}";
    }

    public string Accept(TMap type)
    {
        return $"map[{type.KeyType.Apply(this)}]{type.ValueType.Apply(this)}";
    }

    public string Accept(TDateTime type)
    {
        return "int64";
    }
}
