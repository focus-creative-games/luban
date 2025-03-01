using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Dart.TypeVisitors;

class DartDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static DartDeclaringTypeNameVisitor Ins { get; } = new();

    public virtual string Accept(TBool type)
    {
        return type.IsNullable ? "bool?" : "bool";
    }

    public virtual string Accept(TByte type)
    {
        return type.IsNullable ? "int?" : "int";
    }

    public virtual string Accept(TShort type)
    {
        return type.IsNullable ? "int?" : "int";
    }

    public virtual string Accept(TInt type)
    {
        return type.IsNullable ? "int?" : "int";
    }

    public virtual string Accept(TLong type)
    {
        return type.IsNullable ? "int?" : "int";
    }

    public virtual string Accept(TFloat type)
    {
        return type.IsNullable ? "double?" : "double";
    }

    public virtual string Accept(TDouble type)
    {
        return type.IsNullable ? "double?" : "double";
    }

    public virtual string Accept(TEnum type)
    {
       return type.DefEnum.TypeNameWithTypeMapper() ?? type.DefEnum.Name;
    }

    public string Accept(TString type)
    {
        return "String";
    }

    public virtual string Accept(TDateTime type)
    {
        return type.IsNullable ? "int?" : "int";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.TypeNameWithTypeMapper() ?? type.DefBean.Name;
    }

    public string Accept(TArray type)
    {
        return $"List<{type.ElementType.Apply(DartDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TList type)
    {
        return $"List<{type.ElementType.Apply(DartDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TSet type)
    {
        return $"HashSet<{type.ElementType.Apply(DartDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TMap type)
    {
        return $"Map<{type.KeyType.Apply(DartDeclaringBoxTypeNameVisitor.Ins)}, {type.ValueType.Apply(DartDeclaringBoxTypeNameVisitor.Ins)}>";
    }
}
