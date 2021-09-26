{{typescript_namespace_begin}}
{{~if comment != '' ~}}
/**
 * {{comment}}
 */
{{~end~}}
export enum {{name}} {
    {{~for item in items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
     */
{{~end~}}
    {{item.name}} = {{item.value}},
    {{~end~}}
}
{{typescript_namespace_end}}