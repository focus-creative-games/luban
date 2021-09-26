{{cpp_namespace_begin}}
{{~if comment != '' ~}}
/**
 * {{comment}}
 */
{{~end~}}
enum class {{name}}
{
    {{~ for item in items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
     */
{{~end~}}
    {{item.name}} = {{item.value}},
    {{~end~}}
};
{{cpp_namespace_end}}