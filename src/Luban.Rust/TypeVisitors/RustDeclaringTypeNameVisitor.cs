using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Rust.TypeVisitors;

public class RustDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static readonly RustDeclaringTypeNameVisitor Ins = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "u8";
    }

    public string Accept(TShort type)
    {
        return "i16";
    }

    public string Accept(TInt type)
    {
        return "i32";
    }

    public string Accept(TLong type)
    {
        return "i64";
    }

    public string Accept(TFloat type)
    {
        return "f32";
    }

    public string Accept(TDouble type)
    {
        return "f64";
    }

    public string Accept(TEnum type)
    {
        return $"crate::{(type.DefEnum.TypeNameWithTypeMapper() ?? type.DefEnum.FullName).Replace(".", "::")}";
    }

    public string Accept(TString type)
    {
        return "String";
    }

    public string Accept(TDateTime type)
    {
        return "u64";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.IsAbstractType 
            ? "std::sync::Arc<AbstractBase>" 
            : $"crate::{(type.DefBean.TypeNameWithTypeMapper() ?? type.DefBean.FullName).Replace(".", "::")}";
    }

    public string Accept(TArray type)
    {
        return $"Vec<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TList type)
    {
        return $"Vec<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"std::collections::HashSet<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"std::collections::HashMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }
}