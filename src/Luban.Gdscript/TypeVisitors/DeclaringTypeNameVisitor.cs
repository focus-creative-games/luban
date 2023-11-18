using Luban.Gdscript.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Gdscript.TypeVisitors;

public class DeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static DeclaringTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "int";
    }

    public string Accept(TShort type)
    {
        return "int";
    }

    public string Accept(TInt type)
    {
        return "int";
    }

    public string Accept(TLong type)
    {
        return "int";
    }

    public string Accept(TFloat type)
    {
        return "float";
    }

    public string Accept(TDouble type)
    {
        return "float";
    }

    public virtual string Accept(TEnum type)
    {
        // return GdscriptCommonTemplateExtension.FullName(type.DefEnum);
        return "int";
    }

    public string Accept(TString type)
    {
        return "String";
    }

    public virtual string Accept(TDateTime type)
    {
        return "int";
    }

    public string Accept(TBean type)
    {
        return GdscriptCommonTemplateExtension.FullName(type.DefBean);
    }

    public string Accept(TArray type)
    {
        return $"Array[{type.ElementType.Apply((this))}]";
    }

    public string Accept(TList type)
    {
        return $"Array[{type.ElementType.Apply((this))}]";
    }

    public string Accept(TSet type)
    {
        return $"Array[{type.ElementType.Apply((this))}]";
    }

    public string Accept(TMap type)
    {
        return "Dictionary";
    }
}
