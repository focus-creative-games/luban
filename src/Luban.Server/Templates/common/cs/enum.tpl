
namespace {{namespace_with_top_module}}
{
{{~if comment != '' ~}}
    /// <summary>
    /// {{comment}}
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
        /// {{item.comment}}
        /// </summary>
{{~end~}}
        {{item.name}} = {{item.value}},
        {{~end~}}
    }
}
