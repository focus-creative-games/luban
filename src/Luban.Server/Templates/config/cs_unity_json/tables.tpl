using Bright.Serialization;
using SimpleJSON;
{{
    name = x.name
    namespace = x.namespace
    tables = x.tables
}}
namespace {{namespace}}
{
   
public sealed class {{name}}
{
    {{~for table in tables ~}}
{{~if table.comment != '' ~}}
    /// <summary>
    /// {{table.escape_comment}}
    /// </summary>
{{~end~}}
    public {{table.full_name}} {{table.name}} {get; }
    {{~end~}}

    public {{name}}(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        {{~for table in tables ~}}
        {{table.name}} = new {{table.full_name}}(loader("{{table.output_data_file}}")); 
        tables.Add("{{table.full_name}}", {{table.name}});
        {{~end~}}

        {{~for table in tables ~}}
        {{table.name}}.Resolve(tables); 
        {{~end~}}
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        {{~for table in tables ~}}
        {{table.name}}.TranslateText(translator); 
        {{~end~}}
    }
}

}
