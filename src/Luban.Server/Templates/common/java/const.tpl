
package {{x.namespace_with_top_module}};

{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
public final class {{x.name}}
{
    {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
     */
{{~end~}}
    public static final {{java_define_type item.ctype}} {{item.name}} = {{java_const_value item.ctype item.value}};
    {{~end~}}
}
