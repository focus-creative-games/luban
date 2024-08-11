using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;
class EditorJsonSave : ITypeFuncVisitor<string, string, string, int, string>
{
    public static EditorJsonSave Ins { get; } = new();

    public string Accept(TBool type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONBool({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TByte type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TShort type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TInt type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TLong type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TFloat type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TDouble type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber({value}{(type.IsNullable ? ".Value" : "")});";
    }

    public string Accept(TEnum type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONNumber((int){value});";
    }

    public string Accept(TString type, string jsonName, string jsonFieldName, string value, int depth)
    {
        var ret = "";
        if (type.HasTag("obj"))
        {
            ret += $"{value} = UnityEditor.AssetDatabase.GetAssetPath({value}_UnityObject);";
        }

        ret += $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
        return ret;
    }

    public string Accept(TDateTime type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{jsonName}[\"{jsonFieldName}\"] = new JSONString({value});";
    }

    public string Accept(TBean type, string jsonName, string jsonFieldName, string value, int depth)
    {
        return $"{{ var __bjson = new JSONObject();  {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.SaveJson{type.DefBean.Name}({value}, __bjson); {jsonName}[\"{jsonFieldName}\"] = __bjson; }}";
    }

    public string Accept(TArray type, string jsonName, string jsonFieldName, string value, int depth)
    {
        //return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        var __e = $"__e{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        var __n = $"__n{depth}";
        var __cjson = $"__cjson{depth}";
        return $"{{ var {__cjson} = new JSONArray(); foreach(var {__e} in {value}) {{ {type.ElementType.Apply(this, __cjson, "null", __e, depth++)} }} {jsonName}[\"{jsonFieldName}\"] = {__cjson}; }}";
    }

    public string Accept(TList type, string jsonName, string jsonFieldName, string value, int depth)
    {
        //return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        var __e = $"__e{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        var __n = $"__n{depth}";
        var __cjson = $"__cjson{depth}";
        return $"{{ var {__cjson} = new JSONArray(); foreach(var {__e} in {value}) {{ {type.ElementType.Apply(this, __cjson, "null", __e, depth++)} }} {jsonName}[\"{jsonFieldName}\"] = {__cjson}; }}";
    }

    public string Accept(TSet type, string jsonName, string jsonFieldName, string value, int depth)
    {
        //return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ {type.ElementType.Apply(this, "__cjson", "null", "_e")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        var __e = $"__e{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        var __n = $"__n{depth}";
        var __cjson = $"__cjson{depth}";
        return $"{{ var {__cjson} = new JSONArray(); foreach(var {__e} in {value}) {{ {type.ElementType.Apply(this, __cjson, "null", __e, depth++)} }} {jsonName}[\"{jsonFieldName}\"] = {__cjson}; }}";
    }

    public string Accept(TMap type, string jsonName, string jsonFieldName, string value, int depth)
    {
        //return $"{{ var __cjson = new JSONArray(); foreach(var _e in {value}) {{ var __entry = new JSONArray(); __cjson[null] = __entry; {type.KeyType.Apply(this, "__entry", "null", "_e.Key")} {type.ValueType.Apply(this, "__entry", "null", "_e.Value")} }} {jsonName}[\"{jsonFieldName}\"] = __cjson; }}";
        var __e = $"__e{depth}";
        var __e1 = $"__e1{depth}";
        var __k = $"__k{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        var __n = $"__n{depth}";
        var __cjson = $"__cjson{depth}";
        var __cjson1 = $"__cjson1{depth}";
        var __entry = $"__entry{depth}";

        return $@"
if ({value} == null)
{{
    throw new System.ArgumentNullException();
}}

var {__cjson} = new JSONArray();

foreach (var {__e} in {value})
{{
    var {__entry} = new JSONArray();
    {__cjson}.Add({__entry});

    {__entry}.Add(new JSONNumber((int){__e}[0]));

    var {__cjson1} = new JSONArray();

    foreach (var {__e1} in (List<int>){__e}[1])
    {{
        {__cjson1}.Add(new JSONNumber({__e1}));
    }}

    {__entry}.Add({__cjson1});
}}

{jsonName}[""{jsonFieldName}""] = {__cjson};
";


    }
}
