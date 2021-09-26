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

{{~if x.comment != '' ~}}
/// <summary>
/// {{x.comment}}
/// </summary>
{{~end~}}
public {{x.cs_class_modifier}} class {{name}} : {{if parent_def_type}} {{parent}} {{else}} Bright.Config.BeanBase {{end}}
{
    public {{name}}(JsonElement _json) {{if parent_def_type}} : base(_json) {{end}}
    {
        {{~ for field in export_fields ~}}
        {{cs_json_deserialize '_json' field.cs_style_name field.name field.ctype}}
        {{~if field.index_field~}}
        foreach(var _v in {{field.cs_style_name}}) { {{field.cs_style_name}}_Index.Add(_v.{{field.index_field.cs_style_name}}, _v); }
        {{~end~}}
        {{~end~}}
    }

    public {{name}}({{~for field in hierarchy_export_fields }}{{cs_define_type field.ctype}} {{field.name}}{{if !for.last}},{{end}} {{end}}) {{if parent_def_type}} : base({{- for field in parent_def_type.hierarchy_export_fields }}{{field.name}}{{if !for.last}},{{end}}{{end}}) {{end}}
    {
        {{~ for field in export_fields ~}}
        this.{{field.cs_style_name}} = {{field.name}};
        {{~if field.index_field~}}
        foreach(var _v in {{field.cs_style_name}}) { {{field.cs_style_name}}_Index.Add(_v.{{field.index_field.cs_style_name}}, _v); }
        {{~end~}}
        {{~end~}}
    }

    public static {{name}} Deserialize{{name}}(JsonElement _json)
    {
    {{~if x.is_abstract_type~}}
        switch (_json.GetProperty("__type__").GetString())
        {
        {{~for child in x.hierarchy_not_abstract_children~}}
            case "{{child.name}}": return new {{child.full_name}}(_json);
        {{~end~}}
            default: throw new SerializationException();
        }
    {{~else~}}
        return new {{x.full_name}}(_json);
    {{~end~}}
    }

    {{~ for field in export_fields ~}}
{{~if field.comment != '' ~}}
    /// <summary>
    /// {{field.comment}}
    /// </summary>
{{~end~}}
    public {{cs_define_type field.ctype}} {{field.cs_style_name}} { get; private set; }
    {{~if field.index_field~}} 
    public readonly Dictionary<{{cs_define_type field.index_field.ctype}}, {{cs_define_type field.ctype.element_type}}> {{field.cs_style_name}}_Index = new Dictionary<{{cs_define_type field.index_field.ctype}}, {{cs_define_type field.ctype.element_type}}>();
    {{~end~}}
    {{~if field.gen_ref~}}
    public {{field.cs_ref_validator_define}}
    {{~end~}}
    {{~if field.gen_text_key~}}
    public {{cs_define_text_key_field field}} { get; }
    {{~end~}}
    {{~end~}}

{{~if !x.is_abstract_type~}}
    public const int ID = {{x.id}};
    public override int GetTypeId() => ID;
{{~end~}}

    public {{x.cs_method_modifier}} void Resolve(Dictionary<string, object> _tables)
    {
        {{~if parent_def_type~}}
        base.Resolve(_tables);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_ref~}}
        {{cs_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
        {{cs_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
    }

    public {{x.cs_method_modifier}} void TranslateText(System.Func<string, string, string> translator)
    {
        {{~if parent_def_type~}}
        base.TranslateText(translator);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_text_key~}}
        {{cs_translate_text field 'translator'}}
        {{~else if field.has_recursive_text~}}
        {{cs_recursive_translate_text field 'translator'}}
        {{~end~}}
        {{~end~}}
    }

    public override string ToString()
    {
        return "{{full_name}}{ "
    {{~ for field in hierarchy_export_fields ~}}
        + "{{field.cs_style_name}}:" + {{cs_to_string field.cs_style_name field.ctype}} + ","
    {{~end~}}
        + "}";
    }
    }
}
