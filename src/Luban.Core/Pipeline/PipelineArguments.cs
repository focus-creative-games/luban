using Luban.Schema;

namespace Luban.Pipeline;

public class PipelineArguments
{
    public string Target { get; set; }

    public bool ForceLoadTableDatas { get; set; }

    public List<string> IncludeTags { get; set; }

    public List<string> ExcludeTags { get; set; }

    public List<string> CodeTargets { get; set; }

    public List<string> DataTargets { get; set; }

    public string SchemaCollector { get; set; }

    public LubanConfig Config { get; set; }

    public List<string> OutputTables { get; set; }

    public string TimeZone { get; set; }

    public Dictionary<string, object> CustomArgs { get; set; }

    public Dictionary<string, string> Variants { get; set; }
}
