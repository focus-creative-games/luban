
{{
    name = x.name
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

{{x.typescript_namespace_begin}}
{{~if x.comment != '' ~}}
/**
 * {{x.escape_comment}}
 */
{{~end~}}
export {{if x.is_abstract_type}}abstract {{end}}class {{name}}{{if parent_def_type}} extends {{x.parent}}{{end}} {
{{~if x.is_abstract_type~}}
    static constructorFrom(_json_: any): {{name}}{
        switch (_json_["{{x.json_type_name_key}}"]) {
        {{~ for child in x.hierarchy_not_abstract_children~}}
            case '{{cs_impl_data_type child x}}': return new {{child.full_name}}(_json_)
        {{~end~}}
            default: throw new Error()
        }
    }
{{~end~}}

    constructor(_json_: any) {
        {{~if parent_def_type~}}
        super(_json_)
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if !field.ctype.is_nullable~}}
        if (_json_.{{field.name}} === undefined) { throw new Error() }
        {{~end~}}
        {{ts_json_constructor ('this.' + field.convention_name) ( '_json_.' + field.name) field.ctype}}
        {{~end~}}
    }

    {{~ for field in export_fields ~}}
{{~if field.comment != '' ~}}
    /**
     * {{field.escape_comment}}
     */
{{~end~}}
    {{~if field.gen_text_key~}}
    {{field.convention_name}}: {{ts_define_type field.ctype}}
    {{~else~}}
    readonly {{field.convention_name}}: {{ts_define_type field.ctype}}
    {{~end~}}
    {{~if field.gen_ref~}}
    {{field.ts_ref_validator_define}}
    {{~end~}}
    {{~if field.gen_text_key~}}
    readonly {{ts_define_text_key_field field}}: {{ts_define_type field.ctype}}
    {{~end~}}
    {{~end~}}

    resolve(_tables: Map<string, any>) {
        {{~if parent_def_type~}}
        super.resolve(_tables)
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_ref~}}
        {{ts_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
        {{ts_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
    }

    translateText(translator: any)
    {
        {{~if parent_def_type~}}
        base.translateText(translator);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_text_key~}}
        {{ts_translate_text field 'translator'}}
        {{~else if field.has_recursive_text~}}
        {{ts_recursive_translate_text field 'translator'}}
        {{~end~}}
        {{~end~}}
    }

}

{{x.typescript_namespace_end}}