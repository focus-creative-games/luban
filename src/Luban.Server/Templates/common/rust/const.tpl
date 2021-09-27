{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}

#[allow(non_snake_case)]
pub mod {{x.rust_full_name}} {
    {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
     */
{{~end~}}
	#[allow(dead_code)]
    pub const {{string.upcase item.name}}: {{rust_define_type item.ctype}} = {{rust_const_value item.ctype item.value}};
    {{~end~}}
}
