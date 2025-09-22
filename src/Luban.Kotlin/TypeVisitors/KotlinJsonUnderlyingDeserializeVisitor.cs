using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Kotlin.TypeVisitors;

public class KotlinJsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static KotlinJsonUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string json, string x, int depth)
    {
        return $"{x} = {json}.asBoolean";
    }

    public string Accept(TByte type, string json, string x, int depth)
    {
        return $"{x} = {json}.asByte";
    }

    public string Accept(TShort type, string json, string x, int depth)
    {
        return $"{x} = {json}.asShort";
    }

    public string Accept(TInt type, string json, string x, int depth)
    {
        return $"{x} = {json}.asInt";
    }

    public string Accept(TLong type, string json, string x, int depth)
    {
        return $"{x} = {json}.asLong";
    }

    public string Accept(TFloat type, string json, string x, int depth)
    {
        return $"{x} = {json}.asFloat";
    }

    public string Accept(TDouble type, string json, string x, int depth)
    {
        return $"{x} = {json}.asDouble";
    }

    public string Accept(TEnum type, string json, string x, int depth)
    {
        return $"{x} = {json}.asInt";
    }

    public string Accept(TString type, string json, string x, int depth)
    {
        return $"{x} = {json}.asString";
    }

    public string Accept(TDateTime type, string json, string x, int depth)
    {
        return $"{x} = {json}.asLong";
    }

    public string Accept(TBean type, string json, string x, int depth)
    {
        return $"{x} = {type.DefBean.FullNameWithTopModule}.deserialize({json}.asJsonObject)";
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        string __n = $"__n{depth}";
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __index = $"__index{depth}";
        string typeStr = type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins);
        
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; val {__n} = _json{depth}_.size(); {x} = Array({__n}) {{ {type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}() }}; var {__index} = 0; for ({__e} in _json{depth}_) {{ var {__v}: {typeStr}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {x}[{__index}++] = {__v} }} }}";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; {x} = mutableListOf(); for ({__e} in _json{depth}_) {{ var {__v}: {type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {x}.add({__v}) }} }}";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; {x} = mutableSetOf(); for ({__e} in _json{depth}_) {{ var {__v}: {type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {x}.add({__v}) }} }}";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; {x} = mutableMapOf(); for ({__e} in _json{depth}_) {{ var {__k}: {type.KeyType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, $"{__e}.asJsonArray[0]", __k, depth + 1)}; var {__v}: {type.ValueType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, $"{__e}.asJsonArray[1]", __v, depth + 1)}; {x}[{__k}] = {__v} }} }}";
    }
}