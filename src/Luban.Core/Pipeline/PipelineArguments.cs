namespace Luban.Pipeline;

public class PipelineArguments
{
    public string Target { get; set; }
    
    public List<string> IncludeTags { get; set; }
    
    public List<string> ExcludeTags { get; set; }
    
    public List<string> CodeTargets { get; set; }
    
    public List<string> DataTargets { get; set; }
    
    public string SchemaCollector { get; set; }
    
    public string SchemaPath { get; set; }

    public List<string> OutputTables { get; set; }
    
    public string TimeZone { get; set; }
}