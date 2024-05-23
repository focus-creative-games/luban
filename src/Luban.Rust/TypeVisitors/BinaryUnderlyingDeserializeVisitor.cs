using Luban.Rust;
using Luban.Rust.TemplateExtensions;
using Luban.Rust.TypeVisitors;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class BinaryUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static BinaryUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_bool()";
    }

    public string Accept(TByte type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_byte()";
    }

    public string Accept(TShort type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_short()";
    }

    public string Accept(TInt type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_int()";
    }

    public string Accept(TLong type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_long()";
    }

    public string Accept(TFloat type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_float()";
    }

    public string Accept(TDouble type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_double()";
    }

    public string Accept(TEnum type, string bufName, string fieldName, int depth)
    {
        return type.DefEnum.IsFlags
            ? $"{type.Apply(RustDeclaringTypeNameVisitor.Ins)}::from_bits_truncate({bufName}.read_uint())"
            : $"{bufName}.read_int().into()";
    }

    public string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_string()";
    }

    public string Accept(TDateTime type, string bufName, string fieldName, int depth)
    {
        return $"{bufName}.read_ulong()";
    }

    public string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        return type.DefBean.IsAbstractType
            ? $"{RustCommonTemplateExtension.FullName(type.DefBean)}::new(&mut {bufName})?"
            : $"{type.Apply(RustDeclaringTypeNameVisitor.Ins)}::new(&mut {bufName})?";
    }

    public string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{let {n} = std::cmp::min({bufName}.read_size(), {bufName}.size());let mut {e} = vec![]; for {i} in 0..{n} {{ {e}.push({type.ElementType.Apply(this, bufName, $"{e}", depth + 1)}); }} {e} }}";
    }

    public string Accept(TList type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{let {n} = std::cmp::min({bufName}.read_size(), {bufName}.size());let mut {e} = vec![]; for {i} in 0..{n} {{ {e}.push({type.ElementType.Apply(this, bufName, $"{e}", depth + 1)}); }} {e} }}";
    }

    public string Accept(TSet type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{let {n} = std::cmp::min({bufName}.read_size(), {bufName}.size());let mut {e} = {ConstantStrings.SetType}::default(); for {i} in 0..{n} {{ {e}.insert({type.ElementType.Apply(this, bufName, $"{e}", depth + 1)}); }} {e} }}";
    }

    public string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string k = $"_k{depth}";
        string v = $"_v{depth}";
        string i = $"i{depth}";
        return $"{{let {n} = std::cmp::min({bufName}.read_size(), {bufName}.size()); let mut {e} = {ConstantStrings.MapType}::with_capacity({n} * 3 / 2);for {i} in 0..{n} {{ let {k} = {type.KeyType.Apply(this, bufName, k, depth + 1)}; let {v} = {type.ValueType.Apply(this, bufName, v, depth + 1)}; {e}.insert({k}, {v});}} {e} }}";
    }
}