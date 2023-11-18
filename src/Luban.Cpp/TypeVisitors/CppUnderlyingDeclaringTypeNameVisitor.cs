using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Cpp.TypeVisitors;

public class CppUnderlyingDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static CppUnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "::luban::byte";
    }

    public string Accept(TShort type)
    {
        return "::luban::int16";
    }

    public string Accept(TInt type)
    {
        return "::luban::int32";
    }

    public string Accept(TLong type)
    {
        return "::luban::int64";
    }

    public string Accept(TFloat type)
    {
        return "::luban::float32";
    }

    public string Accept(TDouble type)
    {
        return "::luban::float64";
    }

    public string Accept(TEnum type)
    {
        return CppTemplateExtension.MakeTypeCppName(type.DefEnum);
    }

    public string Accept(TString type)
    {
        return "::luban::String";
    }

    public virtual string Accept(TBean type)
    {
        return CppTemplateExtension.MakeTypeCppName(type.DefBean);
    }

    public string Accept(TDateTime type)
    {
        return "::luban::datetime";
    }

    public string Accept(TArray type)
    {
        return $"::luban::Array<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TList type)
    {
        return $"::luban::Vector<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"::luban::HashSet<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"::luban::HashMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }
}
