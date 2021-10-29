{{typescript_namespace_begin}}
{{~if comment != '' ~}}
/**
 * {{comment | html.escape}}
 */
{{~end~}}
export enum {{name}} {
    {{~for item in items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.escape_comment}}
     */
{{~end~}}
    {{item.name}} = {{item.value}},
    {{~end~}}
}
{{typescript_namespace_end}}