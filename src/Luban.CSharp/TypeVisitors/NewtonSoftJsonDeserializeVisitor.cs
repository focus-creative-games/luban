using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

internal class NewtonSoftJsonDeserializeVisitor: ITypeFuncVisitor<string, string, int, string>
{

    public static NewtonSoftJsonDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string x, string y, int z)
    {
        return $"{y} = (bool){x};";
    }

    public string Accept(TByte type, string x, string y, int z)
    {
        return $"{y} = (byte){x};";
    }

    public string Accept(TShort type, string x, string y, int z)
    {
        return $"{y} = (short){x};";
    }

    public string Accept(TInt type, string x, string y, int z)
    {
        return $"{y} = (int){x};";
    }

    public string Accept(TLong type, string x, string y, int z)
    {
        return $"{y} = (long){x};";
    }

    public string Accept(TFloat type, string x, string y, int z)
    {
        return $"{y} = (float){x};";
    }

    public string Accept(TDouble type, string x, string y, int z)
    {
        return $"{y} = (double){x};";
    }

    public string Accept(TEnum type, string x, string y, int z)
    {
        return $"{y} = ({type.Apply(DeclaringTypeNameVisitor.Ins)})(int){x};";
    }

    public string Accept(TString type, string x, string y, int z)
    {
        return $"{y} = (string){x};";
    }

    public string Accept(TDateTime type, string x, string y, int z)
    {
        return $"{y} = (long){x};";
    }

    public string Accept(TBean type, string x, string y, int z)
    {
        string src = $"{CSharpUtil.GetFullNameWithGlobalQualifier(type.DefBean)}.Deserialize{type.DefBean.Name}({x})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{y} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        string _n = $"_n{depth}";
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __json = $"__json{depth}";
        string __index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}[{_n}]";
        if (type.Dimension > 1)
        {
            typeStr = $"{type.FinalElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}[{_n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        return $"{{ var {__json} = {json}; int {_n} = ({__json} as JArray).Count; {x} = new {typeStr}; int {__index}=0; foreach(JToken {__e} in {__json}) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}[{__index}++] = {__v}; }}   }}";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __json = $"__json{depth}";
        return $"{{ var {__json} = {json}; {x} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(({__json} as JArray).Count); foreach(JToken {__e} in {__json}) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}.Add({__v}); }}   }}";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __json = $"__json{depth}";
        return $"{{ var {__json} = {json}; {x} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(({__json} as JArray).Count); foreach(JToken {__e} in {__json}) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}.Add({__v}); }}   }}";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        string __e = $"__e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        string __json = $"__json{depth}";
        return @$"{{ var {__json} = {json}; {x} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(({__json} as JArray).Count); foreach(JToken {__e} in {__json}) {{ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} {__k};  {type.KeyType.Apply(this, $"{__e}[0]", __k, depth + 1)} {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth + 1)}  {x}.Add({__k}, {__v}); }}   }}";
    }
}
