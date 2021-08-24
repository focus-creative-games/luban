{{x.typescript_namespace_begin}}
{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
export class {{x.name}} {
    {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
     */
{{~end~}}
    static {{item.name}} = {{ts_const_value item.ctype item.value}};
    {{~end~}}
}
{{x.typescript_namespace_end}}
