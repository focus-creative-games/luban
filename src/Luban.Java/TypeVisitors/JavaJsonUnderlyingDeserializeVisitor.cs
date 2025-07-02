using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Java.TypeVisitors;

public class JavaJsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static JavaJsonUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsBoolean();";
    }

    public string Accept(TByte type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsByte();";
    }

    public string Accept(TShort type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsShort();";
    }

    public string Accept(TInt type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsInt();";
    }

    public string Accept(TLong type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsLong();";
    }

    public string Accept(TFloat type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsFloat();";
    }

    public string Accept(TDouble type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsDouble();";
    }

    public string Accept(TEnum type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsInt();";
    }

    public string Accept(TString type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsString();";
    }

    public string Accept(TDateTime type, string json, string x, int depth)
    {
        return $"{x} = {json}.getAsLong();";
    }

    public string Accept(TBean type, string json, string x, int depth)
    {
        return $"{x} = {type.DefBean.FullNameWithTopModule}.deserialize({json}.getAsJsonObject());";
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        string __n = $"__n{depth}";
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)}[{__n}]";
        if (type.Dimension > 1)
        {
            typeStr = $"{type.FinalElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)}[{__n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        return $"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); int {__n} = _json{depth}_.size(); {x} = new {typeStr}; int {__index}=0; for(JsonElement {__e} : _json{depth}_) {{ {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}[{__index}++] = {__v}; }}   }}";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        return $"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); {x} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(_json{depth}_.size()); for(JsonElement {__e} : _json{depth}_) {{ {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  {x}.add({__v}); }}   }}";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        return $"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); {x} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(_json{depth}_.size()); for(JsonElement {__e} : _json{depth}_) {{ {type.ElementType.Apply(JavaDeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  {x}.add({__v}); }}   }}";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        return @$"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); {x} = new {type.Apply(JavaDeclaringTypeNameVisitor.Ins)}(_json{depth}_.size()); for(JsonElement {__e} : _json{depth}_) {{ {type.KeyType.Apply(JavaDeclaringTypeNameVisitor.Ins)} {__k};  {type.KeyType.Apply(this, $"{__e}.getAsJsonArray().get(0)", __k, depth + 1)} {type.ValueType.Apply(JavaDeclaringTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}.getAsJsonArray().get(1)", __v, depth + 1)}  {x}.put({__k}, {__v}); }}   }}";
    }
}
