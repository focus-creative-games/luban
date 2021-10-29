
namespace {{namespace_with_top_module}}
{
{{~if comment != '' ~}}
    /// <summary>
    /// {{comment | html.escape}}
    /// </summary>
{{~end~}}
    {{~if is_flags~}}
    [System.Flags]
    {{~end~}}
    public enum {{name}}
    {
        {{~ for item in items ~}}
{{~if item.comment != '' ~}}
        /// <summary>
        /// {{item.escape_comment}}
        /// </summary>
{{~end~}}
        {{item.name}} = {{item.value}},
        {{~end~}}
    }
}
