package {{x.namespace_with_top_module}};

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

{{
    name = x.name
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
public {{x.java_class_modifier}} class {{name}}{{if parent_def_type}} extends {{x.parent_def_type.full_name_with_top_module}}{{end}} {
    public {{name}}(JsonObject __json__) { 
        {{~if parent_def_type~}}
        super(__json__);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{java_json_deserialize '__json__' field.java_style_name field.name field.ctype}}
        {{~if field.index_field~}}
        for({{java_box_define_type field.ctype.element_type}} _v : {{field.java_style_name}}) {
            {{field.java_style_name}}_Index.put(_v.{{field.index_field.java_style_name}}, _v); 
        }
        {{~end~}}
        {{~end~}}
    }

    public {{name}}({{- for field in hierarchy_export_fields }}{{java_define_type field.ctype}} {{field.name}}{{if !for.last}},{{end}} {{end}}) {
        {{~if parent_def_type~}}
        super({{ for field in parent_def_type.hierarchy_export_fields }}{{field.name}}{{if !for.last}}, {{end}}{{end}});
        {{~end~}}
        {{~ for field in export_fields ~}}
        this.{{field.java_style_name}} = {{field.name}};
        {{~if field.index_field~}}
        for({{java_box_define_type field.ctype.element_type}} _v : {{field.java_style_name}}) {
            {{field.java_style_name}}_Index.put(_v.{{field.index_field.java_style_name}}, _v); 
        }
        {{~end~}}
        {{~end~}}
    }

    public static {{name}} deserialize{{name}}(JsonObject __json__) {
    {{~if x.is_abstract_type~}}
        switch (__json__.get("__type__").getAsString()) {
        {{~for child in x.hierarchy_not_abstract_children~}}
            case "{{child.name}}": return new {{child.full_name_with_top_module}}(__json__);
        {{~end~}}
            default: throw new bright.serialization.SerializationException();
        }
    {{~else~}}
        return new {{name}}(__json__);
    {{~end~}}
    }

    {{~ for field in export_fields ~}}
{{~if field.comment != '' ~}}
    /**
     * {{field.comment}}
     */
{{~end~}}
    public final {{java_define_type field.ctype}} {{field.java_style_name}};
    {{~if field.index_field~}} 
    public final java.util.HashMap<{{java_box_define_type field.index_field.ctype}}, {{java_box_define_type field.ctype.element_type}}> {{field.java_style_name}}_Index = new java.util.HashMap<>();
    {{~end~}}
    {{~if field.gen_ref~}}
    public {{field.java_ref_validator_define}}
    {{~end~}}
    {{~end~}}

{{~if !x.is_abstract_type && x.parent_def_type~}}
    public static final int ID = {{x.id}};

    @Override
    public int getTypeId() { return ID; }
{{~else if x.is_abstract_type && !x.parent_def_type~}}
    public abstract int getTypeId();
{{~end~}}

    {{~if parent_def_type~}}
    @Override
    {{~end~}}
    public void resolve(java.util.HashMap<String, Object> _tables) {
        {{~if parent_def_type~}}
        super.resolve(_tables);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_ref~}}
        {{java_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
        {{java_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
    }

    @Override
    public String toString() {
        return "{{full_name}}{ "
    {{~for field in hierarchy_export_fields ~}}
        + "{{field.java_style_name}}:" + {{java_to_string field.java_style_name field.ctype}} + ","
    {{~end~}}
        + "}";
    }
}
