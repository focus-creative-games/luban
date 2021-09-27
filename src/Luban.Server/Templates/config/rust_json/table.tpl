{{ 
    name = x.rust_full_name
    key_type = x.key_ttype
    value_type =  x.value_ttype
}}
{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
#[allow(non_camel_case_types)]
pub struct {{name}} {
    {{~if x.is_map_table ~}}
    data_list: Vec<{{rust_define_type value_type}}>,
    data_map: std::collections::HashMap<{{rust_define_type key_type}}, {{rust_define_type value_type}}>,
    {{~else~}}
    data: {{rust_define_type value_type}},
    {{~end~}}
}

impl {{name}}{
    pub fn new(__js: &json::JsonValue) -> Result<std::rc::Rc<{{name}}>, LoadError> {
    {{~if x.is_map_table ~}}
        if !__js.is_array() {
            return Err(LoadError{});
        }
        let mut t = {{name}} {
            data_list : Vec::new(),
            data_map: std::collections::HashMap::new(),
        };
        
        for __e in __js.members() {
            let __v = match {{rust_class_name value_type}}::new(__e) {
                Ok(x) => x,
                Err(err) => return Err(err),
            };
            let __v2 = std::rc::Rc::clone(&__v);
            t.data_list.push(__v);
            t.data_map.insert(__v2.{{x.index_field.rust_style_name}}.clone(), __v2);
        }
        Ok(std::rc::Rc::new(t))
    }
    #[allow(dead_code)]
    pub fn get_data_map(self:&{{name}}) -> &std::collections::HashMap<{{rust_define_type key_type}}, {{rust_define_type value_type}}> { &self.data_map }
    #[allow(dead_code)]
    pub fn get_data_list(self:&{{name}}) -> &Vec<{{rust_define_type value_type}}> { &self.data_list }
    #[allow(dead_code)]
    pub fn get(self:&{{name}}, key: {{rust_define_type key_type}}) -> std::option::Option<&{{rust_define_type value_type}}> { self.data_map.get(&key) }
    {{~else~}}
        if !__js.is_array() || __js.len() != 1 {
            return Err(LoadError{});
        }
        let __v = match {{rust_class_name value_type}}::new(&__js[0]) {
            Ok(x) => x,
            Err(err) => return Err(err),
        };
        let t = {{name}} {
            data: __v,
        };
        Ok(std::rc::Rc::new(t))
    }
    #[allow(dead_code)]
    pub fn get_data(self:&{{name}}) -> &{{rust_define_type value_type}} { &self.data }
    {{~end~}}
}