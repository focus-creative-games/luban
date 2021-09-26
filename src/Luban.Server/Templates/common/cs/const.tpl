
namespace {{x.namespace_with_top_module}}
{
{{~if x.comment != '' ~}}
    /// <summary>
    /// {{x.comment}}
    /// </summary>
{{~end~}}
    public sealed class {{x.name}}
    {
        {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
        /// <summary>
        /// {{item.comment}}
        /// </summary>
{{~end~}}
        public const {{cs_define_type item.ctype}} {{item.name}} = {{cs_const_value item.ctype item.value}};
        {{~end~}}
    }
}

