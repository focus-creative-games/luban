namespace Luban;

public class GenerationArguments
{
    public string Target { get; set; }
    
    public string OutputTables { get; set; }

    public string OutputIncludeTables { get; set; }
    
    public string OutputExcludeTables { get; set; }

    public TimeZoneInfo TimeZone { get; set; }

    public List<string> ExcludeTags { get; set; }
    
    public List<string> CodeTargets { get; set; }
    
    public List<string> DataTargets { get; set; }
    
    public string SchemaCollector { get; set; }
    
    public string SchemaPath { get; set; }

    public List<string> Tables { get; set; }
    
    public List<string> IncludedTables { get; set; }
    
    public List<string> ExcludedTables { get; set; }

    public Dictionary<string, string> GeneralArgs { get; set; }

    public string GetOption(string family, string name, bool useGlobalIfNotExits)
    {
        string nameWithFamily = family + "." + name;
        if (GeneralArgs.TryGetValue(nameWithFamily, out var value))
        {
            return value;
        }
        if (useGlobalIfNotExits && GeneralArgs.TryGetValue(name, out value))
        {
            return value;
        }
        throw new Exception($"option '{nameWithFamily}' not exists");
    }
    
    public bool TryGetOption(string family, string name, bool useGlobalIfNotExits, out string value)
    {
        string nameWithFamily = family + "." + name;
        if (GeneralArgs.TryGetValue(nameWithFamily, out value))
        {
            return true;
        }
        if (useGlobalIfNotExits && GeneralArgs.TryGetValue("global." + name, out value))
        {
            return true;
        }

        return false;
    }
}