{{cpp_namespace_begin}}
{{~if comment != '' ~}}
/**
 * {{comment | html.escape}}
 */
{{~end~}}
enum class {{name}}
{
    {{~ for item in items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.escape_comment}}
     */
{{~end~}}
    {{item.name}} = {{item.value}},
    {{~end~}}
};
{{cpp_namespace_end}}