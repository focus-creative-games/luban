using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

class EditorJsonLoad : ITypeFuncVisitor<string, string, string>
{
    public static EditorJsonLoad Ins { get; } = new();

    public string Accept(TBool type, string json, string x)
    {
        return $"if(!{json}.IsBoolean) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TByte type, string json, string x)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TShort type, string json, string x)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TInt type, string json, string x)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TLong type, string json, string x)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TFloat type, string json, string x)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TDouble type, string json, string x)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TEnum type, string json, string x)
    {
        return $"if({json}.IsString) {{ {x} = ({type.Apply(EditorDeclaringTypeNameVisitor.Ins)})System.Enum.Parse(typeof({type.Apply(EditorDeclaringTypeNameVisitor.Ins)}), {json}); }} else if({json}.IsNumber) {{ {x} = ({type.Apply(EditorDeclaringTypeNameVisitor.Ins)})(int){json}; }} else {{ throw new SerializationException(); }}  ";
    }

    public string Accept(TString type, string json, string x)
    {
        return $"if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TBean type, string json, string x)
    {
        return $"if(!{json}.IsObject) {{ throw new SerializationException(); }}  {x} = {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.LoadJson{type.DefBean.Name}({json});";
    }

    public string Accept(TArray type, string json, string x)
    {
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} int _n = {json}.Count; {x} = new {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}[_n]; int _index=0; foreach(SimpleJSON.JSONNode __e in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}[_index++] = __v; }}  ";
    }

    public string Accept(TList type, string json, string x)
    {
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(JSONNode __e in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.Add(__v); }}  ";
    }

    public string Accept(TSet type, string json, string x)
    {
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(JSONNode __e in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.Add(__v); }}  ";
    }

    public string Accept(TMap type, string json, string x)
    {
        return @$"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(JSONNode __e in {json}.Children) {{ {type.KeyType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __k;  {type.KeyType.Apply(this, "__e[0]", "__k")} {type.ValueType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v;  {type.ValueType.Apply(this, "__e[1]", "__v")}  {x}.Add(__k, __v); }}  ";
    }

    public string Accept(TDateTime type, string json, string x)
    {
        return $"if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json};";
    }
}
