using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;

{{ 
    name = x.name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
}}

namespace {{x.namespace_with_top_module}}
{

{{~if x.comment != '' ~}}
/// <summary>
/// {{x.escape_comment}}
/// </summary>
{{~end~}}
public sealed class {{name}}
{
    {{~if x.is_map_table ~}}
    private readonly Dictionary<{{cs_define_type key_type}}, {{cs_define_type value_type}}> _dataMap;
    private readonly List<{{cs_define_type value_type}}> _dataList;
    
    public {{name}}(JSONNode _json)
    {
        _dataMap = new Dictionary<{{cs_define_type key_type}}, {{cs_define_type value_type}}>();
        _dataList = new List<{{cs_define_type value_type}}>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = {{cs_define_type value_type}}.Deserialize{{value_type.bean.name}}(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.{{x.index_field.convention_name}}, _v);
        }
    }

    public Dictionary<{{cs_define_type key_type}}, {{cs_define_type value_type}}> DataMap => _dataMap;
    public List<{{cs_define_type value_type}}> DataList => _dataList;

{{~if value_type.is_dynamic~}}
    public T GetOrDefaultAs<T>({{cs_define_type key_type}} key) where T : {{cs_define_type value_type}} => _dataMap.TryGetValue(key, out var v) ? (T)v : null;
    public T GetAs<T>({{cs_define_type key_type}} key) where T : {{cs_define_type value_type}} => (T)_dataMap[key];
{{~end~}}
    public {{cs_define_type value_type}} GetOrDefault({{cs_define_type key_type}} key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public {{cs_define_type value_type}} Get({{cs_define_type key_type}} key) => _dataMap[key];
    public {{cs_define_type value_type}} this[{{cs_define_type key_type}} key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
        {{~else if x.is_list_table ~}}
    private readonly List<{{cs_define_type value_type}}> _dataList;
    
    public {{name}}(JSONNode _json)
    {
        _dataList = new List<{{cs_define_type value_type}}>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = {{cs_define_type value_type}}.Deserialize{{value_type.bean.name}}(_row);
            _dataList.Add(_v);
        }
    }

    public List<{{cs_define_type value_type}}> DataList => _dataList;

    public {{cs_define_type value_type}} Get(int index) => _dataList[index];
    public {{cs_define_type value_type}} this[int index] => _dataList[index];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }

    {{~else~}}

     private readonly {{cs_define_type value_type}} _data;

    public {{name}}(JSONNode _json)
    {
        if(!_json.IsArray)
        {
            throw new SerializationException();
        }
        if (_json.Count != 1) throw new SerializationException("table mode=one, but size != 1");
        _data = {{cs_define_type value_type}}.Deserialize{{value_type.bean.name}}(_json[0]);
    }

    {{~ for field in value_type.bean.hierarchy_export_fields ~}}
{{~if field.comment != '' ~}}
    /// <summary>
    /// {{field.escape_comment}}
    /// </summary>
{{~end~}}
     public {{cs_define_type field.ctype}} {{field.convention_name}} => _data.{{field.convention_name}};
    {{~if field.ref~}}
        public {{field.cs_ref_type_name}} {{field.ref_var_name}} => _data.{{field.ref_var_name}};
    {{~end~}}
    {{~end~}}

    public void Resolve(Dictionary<string, object> _tables)
    {
        _data.Resolve(_tables);
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        _data.TranslateText(translator);
    }

    {{~end~}}
}

}