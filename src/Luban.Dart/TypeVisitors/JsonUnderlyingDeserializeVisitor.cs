using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Dart.TypeVisitors;

class JsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static JsonUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string x, string y, int z)
    {
        return $"{y} = {x} as bool";
    }

    public string Accept(TByte type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TShort type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TInt type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TLong type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TFloat type, string x, string y, int z)
    {
        return $"{y} = {x}.toDouble()";
    }

    public string Accept(TDouble type, string x, string y, int z)
    {
        return $"{y} = {x}.toDouble()";
    }

    public string Accept(TEnum type, string x, string y, int z)
    {
        var name = type.DefEnum.Name;
        return $"{y} = {name}.fromValue({x})";
    }

    public string Accept(TString type, string x, string y, int z)
    {
        return $"{y} = {x} as String";
    }

    public string Accept(TDateTime type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TBean type, string x, string y, int z)
    {
        return $"{y} = {type.DefBean.Name}.deserialize({x})";
    }

    public string Accept(TArray type, string x, string y, int depth)
    {
        string __j = $"_json{depth}";
        string __v = $"_v{depth}";
        string __e = $"_e{depth}";
        return $"{{var {__j} = {x} as List<dynamic>; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}.empty(growable: true); for(var {__e} in {__j}) {{{type.ElementType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {y}.add({__v}); }} }}";
    }

    public string Accept(TList type, string x, string y, int depth)
    {
        string __j = $"_json{depth}";
        string __v = $"_v{depth}";
        string __e = $"_e{depth}";
        return $"{{var {__j} = {x} as List<dynamic>; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}.empty(growable: true); for(var {__e} in {__j}) {{{type.ElementType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {y}.add({__v}); }} }}";
    }

    public string Accept(TSet type, string x, string y, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        string __j = $"_json{depth}";

        return $"{{var {__j} = {x} as List<dynamic>; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}(); for(var {__e} in {__j}) {{{type.ElementType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v}; {type.ElementType.Apply(this, __e, __v, depth + 1)};  {y}.add({__v}); }} }}";
    }

    public string Accept(TMap type, string x, string y, int depth)
    {
        string __e = $"__e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        string __json = $"__json{depth}";

        return @$"{{ var {__json} = {x}; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}(); for(var {__e} in {__json}) {{ var {type.KeyType.Apply(this, $"{__e}[0]", __k, depth + 1)}; {type.ValueType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth + 1)};  {y}[{__k}]= {__v}; }}   }}";
    }
}
