namespace Luban.RawDefs;

public class RawTarget
{
    public string Name { get; set; }

    public string Manager { get; set; }

    public string TopModule { get; set; }

    public List<string> Groups { get; set; } = new();
}
