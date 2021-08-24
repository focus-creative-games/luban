
{{x.cpp_namespace_begin}}
{{~if comment != '' ~}}
/**
 * {{comment}}
 */
{{~end~}}
struct {{x.name}}
{
    {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
    */
{{~end~}}
    static constexpr {{cpp_define_type item.ctype}} {{item.name}} = {{cpp_const_value item.ctype item.value}};
    {{~end~}}
};
{{x.cpp_namespace_end}}
