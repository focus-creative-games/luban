using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

class EditorJsonSaveUnderlying : ITypeFuncVisitor<string, string, string, int, string>
{
    public static EditorJsonSaveUnderlying Ins { get; } = new();

    public string Accept(TBool type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONBool({value});";
    }

    public string Accept(TByte type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value});";
    }

    public string Accept(TShort type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value});";
    }

    public string Accept(TInt type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value});";
    }

    public string Accept(TLong type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value});";
    }

    public string Accept(TFloat type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value});";
    }

    public string Accept(TDouble type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value});";
    }

    public string Accept(TEnum type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber((int){value});";
    }

    public string Accept(TString type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
    }

    public string Accept(TDateTime type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
    }

    public string Accept(TBean type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{{ var __bjson{depth} = new JSONObject(); {jsonName}[\"{jsonFieldName}\"] = __bjson{depth}; {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.SaveJson{type.DefBean.Name}({value}, __bjson{depth}); }}";
    }

    private string AcceptArrayLink(TType elementType, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{{ var __cjson{depth} = new JSONArray(); {jsonName}[\"{jsonFieldName}\"] = __cjson{depth}; foreach(var _e{depth} in {value}) {{ JSONNode __v{depth}; {elementType.Apply(EditorJsonSaveUnderlying2.Ins, $"__v{depth}", $"_e{depth}", depth + 1)} __cjson{depth}.Add(__v{depth}); }} }}";
    }

    public string Accept(TArray type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return AcceptArrayLink(type.ElementType, jsonName, jsonFieldName, value, depth);
    }

    public string Accept(TList type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return AcceptArrayLink(type.ElementType, jsonName, jsonFieldName, value, depth);
    }

    public string Accept(TSet type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return AcceptArrayLink(type.ElementType, jsonName, jsonFieldName, value, depth);
    }

    public string Accept(TMap type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{{ var __cjson{depth} = new JSONArray(); {jsonName}[\"{jsonFieldName}\"] = __cjson{depth}; foreach(var _e{depth} in {value}) {{ var __entry{depth} = new JSONArray(); __cjson{depth}.Add(__entry{depth}); JSONNode __k{depth}; {type.KeyType.Apply(EditorJsonSaveUnderlying2.Ins, $"__k{depth}", $"_e{depth}.Key", depth + 1)} __entry{depth}.Add(__k{depth}); JSONNode __v{depth}; {type.ValueType.Apply(EditorJsonSaveUnderlying2.Ins, $"__v{depth}", $"_e{depth}.Value", depth + 1)} __entry{depth}.Add(__v{depth}); }}  }}";
    }
}
