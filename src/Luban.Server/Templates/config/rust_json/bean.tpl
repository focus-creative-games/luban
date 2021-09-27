
{{
    name = x.rust_full_name
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
#[allow(non_camel_case_types)]
pub struct {{name}} {
{{~for field in hierarchy_export_fields~}}
pub {{field.rust_style_name}}: {{rust_define_type field.ctype}},
{{~end~}}
}

impl {{name}} {
    #[allow(dead_code)]
    pub fn new(__js: &json::JsonValue) -> Result<std::rc::Rc<{{name}}>, LoadError> {
        let __b = {{name}} {
{{~for field in hierarchy_export_fields~}}
            {{field.rust_style_name}}: {{rust_json_constructor ('__js["' + field.name + '"]') field.ctype}},
{{~end~}}
        };
        Ok(std::rc::Rc::new(__b))
    }
}
