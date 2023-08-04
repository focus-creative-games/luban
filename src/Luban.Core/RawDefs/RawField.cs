namespace Luban.RawDefs;

public class RawField
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public string Comment { get; set; }

    public string Tags { get; set; }

    public bool IgnoreNameValidation { get; set; }
    
    public List<string> Groups { get; set; }
}