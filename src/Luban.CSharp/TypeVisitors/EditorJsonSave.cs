using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

class EditorJsonSave : ITypeFuncVisitor<string, string, string, string>
{
    public static EditorJsonSave Ins { get; } = new();

    public string Accept(TBool type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONBool({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TByte type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TShort type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TInt type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TLong type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TFloat type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TDouble type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TEnum type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber((int){value});";
    }

    public string Accept(TString type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
    }

    public string Accept(TDateTime type, string jsonName, string jsonFieldName, string value)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
    }

    public string Accept(TBean type, string jsonName, string jsonFieldName, string value)
    {
        return $"{{ var __bjson = new JSONObject();  {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.SaveJson{type.DefBean.Name}({value}, __bjson); {jsonName}[\"{jsonFieldName}\"] = __bjson; }}";
    }

    public string Accept(TArray type, string jsonName, string jsonFieldName, string value)
    {
        return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
    }

    public string Accept(TList type, string jsonName, string jsonFieldName, string value)
    {
        return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
    }

    public string Accept(TSet type, string jsonName, string jsonFieldName, string value)
    {
        return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
    }

    public string Accept(TMap type, string jsonName, string jsonFieldName, string value)
    {
        return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ var __entry = new JSONArray(); __cjson[null] = __entry; {type.KeyType.Apply(this, "__entry", "null", "_e.Key")} {type.ValueType.Apply(this, "__entry", "null", "_e.Value")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
    }
}
