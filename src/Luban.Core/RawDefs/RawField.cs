namespace Luban.RawDefs;

public class RawField
{
    public string Name { get; set; }

    public string Alias { get; set; }

    public string Type { get; set; }

    public string Comment { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public List<string> Variants { get; set; }

    public bool NotNameValidation { get; set; }

    public List<string> Groups { get; set; }
}
