using Luban.Job.Cfg.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class CsJsonCodeRender : CsCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
using Bright.Serialization;
using System.Collections.Generic;
using System.Text.Json;

{{
    name = x.name
    parent_def_type = x.parent_def_type
    parent = x.parent
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

namespace {{x.namespace_with_top_module}}
{
   
public {{x.cs_class_modifier}} partial class {{name}} : {{if parent_def_type}} {{parent}} {{else}} Bright.Config.BeanBase {{end}}
{
    public {{name}}(JsonElement _buf) {{if parent_def_type}} : base(_buf) {{end}}
    {
        {{~ for field in export_fields ~}}
        {{cs_json_deserialize '_buf' field.cs_style_name field.name field.ctype}}
        {{~if field.index_field~}}
        foreach(var _v in {{field.cs_style_name}}) { {{field.cs_style_name}}_Index.Add(_v.{{field.index_field.cs_style_name}}, _v); }
        {{~end~}}
        {{~end~}}
    }

    public {{name}}({{- for field in hierarchy_export_fields }}{{cs_define_type field.ctype}} {{field.name}}{{if !for.last}},{{end}} {{end}}) {{if parent_def_type}} : base({{- for field in parent_def_type.hierarchy_export_fields }}{{field.name}}{{if !for.last}},{{end}}{{end}}) {{end}}
    {
        {{- for field in export_fields }}
        this.{{field.cs_style_name}} = {{field.name}};
        {{-if field.index_field}}
        foreach(var _v in {{field.cs_style_name}}) { {{field.cs_style_name}}_Index.Add(_v.{{field.index_field.cs_style_name}}, _v); }
        {{-end}}
        {{-end}}
    }

    public static {{name}} Deserialize{{name}}(JsonElement _buf)
    {
    {{if x.is_abstract_type}}
        if (_buf.ValueKind == JsonValueKind.Null) { return null; }
        switch (_buf.GetProperty(""__type__"").GetString())
        {
        {{- for child in x.hierarchy_not_abstract_children}}
            case ""{{child.name}}"": return new {{child.full_name}}(_buf);
        {{-end}}
            default: throw new SerializationException();
        }
    {{else}}
        return new {{x.full_name}}(_buf);
    {{end}}
    }

    {{~ for field in export_fields ~}}
     public readonly {{cs_define_type field.ctype}} {{field.cs_style_name}};
    {{~if field.index_field~}} 
    public readonly Dictionary<{{cs_define_type field.index_field.ctype}}, {{cs_define_type field.ctype.element_type}}> {{field.cs_style_name}}_Index = new Dictionary<{{cs_define_type field.index_field.ctype}}, {{cs_define_type field.ctype.element_type}}>();
    {{~end~}}
    {{~if field.gen_ref~}}
        public {{field.cs_ref_validator_define}}
    {{~end~}}
    {{~end~}}

{{if !x.is_abstract_type}}
    public const int ID = {{x.id}};
    public override int GetTypeId() => ID;
{{end}}

    public {{x.cs_method_modifier}} void Resolve(Dictionary<string, object> _tables)
    {
        {{~if parent_def_type}}base.Resolve(_tables);{{end}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_ref~}}
            {{cs_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
            {{cs_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
        OnResolveFinish(_tables);
    }

    partial void OnResolveFinish(Dictionary<string, object> _tables);

    public override string ToString()
    {
        return ""{{full_name}}{ ""
    {{- for field in hierarchy_export_fields }}
        + ""{{field.cs_style_name}}:"" + {{cs_to_string field.cs_style_name field.ctype}} + "",""
    {{-end}}
        + ""}"";
    }
    }
}

");
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
using Bright.Serialization;
using System.Collections.Generic;
using System.Text.Json;

{{ 
    name = x.name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
}}

namespace {{x.namespace_with_top_module}}
{
public sealed partial class {{name}}
{
    {{~ if x.is_two_key_map_table ~}}
    private readonly Dictionary<{{cs_define_type key_type1}}, List<{{cs_define_type value_type}}>> _dataListMap;
    private readonly Dictionary<{{cs_define_type key_type1}}, Dictionary<{{cs_define_type key_type2}}, {{cs_define_type value_type}}>> _dataMapMap;
    private readonly List<{{cs_define_type value_type}}> _dataList;
    public {{name}}(JsonElement _buf)
    {
      _dataListMap = new Dictionary<{{cs_define_type key_type1}}, List<{{cs_define_type value_type}}>>();
        _dataMapMap = new Dictionary<{{cs_define_type key_type1}}, Dictionary<{{cs_define_type key_type2}}, {{cs_define_type value_type}}>>();
        _dataList = new List<{{cs_define_type value_type}}>();
        
        foreach(JsonElement _row in _buf.EnumerateArray())
        {
            var _v = {{cs_define_type value_type}}.Deserialize{{value_type.bean.name}}(_row);
            _dataList.Add(_v);
            var _key = _v.{{x.index_field1.cs_style_name}};
            if (!_dataListMap.TryGetValue(_key, out var list))
            {
                list = new List<{{cs_define_type value_type}}>();
                _dataListMap.Add(_key, list);
            }
            list.Add(_v);
            if (!_dataMapMap.TryGetValue(_key, out var map))
            {
                map = new Dictionary<{{cs_define_type key_type2}}, {{cs_define_type value_type}}>();
                _dataMapMap.Add(_key, map);
            }
            map.Add(_v.{{x.index_field2.cs_style_name}}, _v);
        }
    }

    public Dictionary<{{cs_define_type key_type1}}, List<{{cs_define_type value_type}}>> DataListMap => _dataListMap;
    public Dictionary<{{cs_define_type key_type1}}, Dictionary<{{cs_define_type key_type2}}, {{cs_define_type value_type}}>> DataMapMap => _dataMapMap;
    public List<{{cs_define_type value_type}}> DataList => _dataList;

    {{if value_type.is_dynamic}}
    public T GetOrDefaultAs<T>({{cs_define_type key_type1}} key1, {{cs_define_type key_type2}} key2) where T : {{cs_define_type value_type}} => _dataMapMap.TryGetValue(key1, out var m) && m.TryGetValue(key2, out var v) ? (T)v : null;
    public T GetAs<T>({{cs_define_type key_type1}} key1, {{cs_define_type key_type2}} key2) where T : {{cs_define_type value_type}} => (T)_dataMapMap[key1][key2];
    {{end}}
    public {{cs_define_type value_type}} GetOrDefault({{cs_define_type key_type1}} key1, {{cs_define_type key_type2}} key2) => _dataMapMap.TryGetValue(key1, out var m) && m.TryGetValue(key2, out var v) ? v : null;
    public {{cs_define_type value_type}} Get({{cs_define_type key_type1}} key1, {{cs_define_type key_type2}} key2) => _dataMapMap[key1][key2];
    public {{cs_define_type value_type}} this[{{cs_define_type key_type1}} key1, {{cs_define_type key_type2}} key2] => _dataMapMap[key1][key2];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        OnResolveFinish(_tables);
    }
    {{~else if x.is_map_table ~}}
    private readonly Dictionary<{{cs_define_type key_type}}, {{cs_define_type value_type}}> _dataMap;
    private readonly List<{{cs_define_type value_type}}> _dataList;
    
    public {{name}}(JsonElement _buf)
    {
        _dataMap = new Dictionary<{{cs_define_type key_type}}, {{cs_define_type value_type}}>();
        _dataList = new List<{{cs_define_type value_type}}>();
        
        foreach(JsonElement _row in _buf.EnumerateArray())
        {
            var _v = {{cs_define_type value_type}}.Deserialize{{value_type.bean.name}}(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.{{x.index_field.cs_style_name}}, _v);
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
        OnResolveFinish(_tables);
    }

    {{~else~}}

     private readonly {{cs_define_type value_type}} _data;

    public {{name}}(JsonElement _buf)
    {
        int n = _buf.GetArrayLength();
        if (n != 1) throw new SerializationException(""table mode=one, but size != 1"");
        _data = {{cs_define_type value_type}}.Deserialize{{value_type.bean.name}}(_buf[0]);
    }


    {{~ for field in value_type.bean.hierarchy_export_fields ~}}
     public {{cs_define_type field.ctype}} {{field.cs_style_name}} => _data.{{field.cs_style_name}};
    {{~if field.ref~}}
        public {{field.cs_ref_type_name}} {{field.cs_ref_var_name}} => _data.{{field.cs_ref_var_name}};
    {{~end~}}
    {{~end~}}

    public void Resolve(Dictionary<string, object> _tables)
    {
        _data.Resolve(_tables);
        OnResolveFinish(_tables);
    }

    {{end}}

    partial void OnResolveFinish(Dictionary<string, object> _tables);
}

}
");
            var result = template.RenderCode(p);

            return result;
        }


        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(@"
using Bright.Serialization;
using System.Text.Json;
{{
    name = x.name
    namespace = x.namespace
    tables = x.tables
}}
namespace {{namespace}}
{
   
public sealed partial class {{name}}
{
    {{- for table in tables }}
    public {{table.full_name}} {{table.name}} {get; }
    {{-end}}

    public {{name}}(System.Func<string, JsonElement> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        {{- for table in tables }}
        {{table.name}} = new {{table.full_name}}(loader(""{{table.json_output_data_file}}"")); 
        tables.Add(""{{table.full_name}}"", {{table.name}});
        {{-end}}

        {{- for table in tables }}
        {{table.name}}.Resolve(tables); 
        {{-end}}
    }
}

}

");
            var result = template.RenderCode(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
