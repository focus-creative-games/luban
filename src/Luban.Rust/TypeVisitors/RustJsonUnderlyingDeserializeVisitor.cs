using Luban.Rust.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Rust.TypeVisitors;

public class RustJsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static readonly RustJsonUnderlyingDeserializeVisitor Ins = new();

    public string Accept(TBool type, string json, string field, int depth)
    {
        return $"{json}.as_bool().unwrap()";
    }

    public string Accept(TByte type, string json, string field, int depth)
    {
        return $"({json}.as_u64().unwrap() as u8)";
    }

    public string Accept(TShort type, string json, string field, int depth)
    {
        return $"({json}.as_i64().unwrap() as i16)";
    }

    public string Accept(TInt type, string json, string field, int depth)
    {
        return $"({json}.as_i64().unwrap() as i32)";
    }

    public string Accept(TLong type, string json, string field, int depth)
    {
        return $"{json}.as_i64().unwrap()";
    }

    public string Accept(TFloat type, string json, string field, int depth)
    {
        return $"({json}.as_f64().unwrap() as f32)";
    }

    public string Accept(TDouble type, string json, string field, int depth)
    {
        return $"{json}.as_f64().unwrap()";
    }

    public string Accept(TEnum type, string json, string field, int depth)
    {
        return type.DefEnum.IsFlags
            ? $"{type.Apply(RustDeclaringTypeNameVisitor.Ins)}::from_bits_truncate(<u32 as std::str::FromStr>::from_str(&{json}.to_string()).unwrap())"
            : $"{json}.as_i64().unwrap().into()";
    }

    public string Accept(TString type, string json, string field, int depth)
    {
        return $"{json}.as_str().unwrap().to_string()";
    }

    public string Accept(TDateTime type, string json, string field, int depth)
    {
        return $"({json}.as_i64().unwrap() as u64)";
    }

    public string Accept(TBean type, string json, string field, int depth)
    {
        return type.DefBean.IsAbstractType
            ? $"{RustCommonTemplateExtension.FullName(type.DefBean)}::new(&{json})?"
            : $"{type.Apply(RustDeclaringTypeNameVisitor.Ins)}::new(&{json})?";
    }

    public string Accept(TArray type, string json, string field, int depth)
    {
        return $"{json}.as_array().unwrap().iter().map(|field| {type.ElementType.Apply(this, "field", "_temp", depth + 1)}).collect()".Replace("?", ".unwrap()");
    }

    public string Accept(TList type, string json, string field, int depth)
    {
        return $"{json}.as_array().unwrap().iter().map(|field| {type.ElementType.Apply(this, "field", "_temp", depth + 1)}).collect()".Replace("?", ".unwrap()");
    }

    public string Accept(TSet type, string json, string field, int depth)
    {
        return $"{json}.as_array().unwrap().iter().map(|field| {type.ElementType.Apply(this, "field", "_temp", depth + 1)}).collect()".Replace("?", ".unwrap()");
    }

    public string Accept(TMap type, string json, string field, int depth)
    {
        return $"std::collections::HashMap::from_iter({json}.as_array().unwrap().iter().map(|x| {{let array = x.as_array().unwrap();({type.KeyType.Apply(this, "array[0]", "", depth + 1)}, {type.ElementType.Apply(this, "array[1]", "", depth + 1)})}}).collect::<Vec<({type.KeyType.Apply(RustDeclaringBoxTypeNameVisitor.Ins)}, {type.ElementType.Apply(RustDeclaringBoxTypeNameVisitor.Ins)})>>())".Replace("?", ".unwrap()");
    }
}