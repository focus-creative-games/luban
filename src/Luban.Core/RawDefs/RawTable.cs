using Luban.Core.Defs;

namespace Luban.Core.RawDefs;

public class RawTable
{
    public string Namespace { get; set; }

    public string Name { get; set; }

    public string Index { get; set; }

    public string ValueType { get; set; }

    public bool LoadDefineFromFile { get; set; }

    public TableMode Mode { get; set; }

    public string Options { get; set; }

    public string Comment { get; set; }

    public string Tags { get; set; }

    public List<string> Groups { get; set; } = new List<string>();

    public List<string> InputFiles { get; set; } = new List<string>();

    public string OutputFile { get; set; }

    public Dictionary<string, List<string>> PatchInputFiles { get; set; } = new Dictionary<string, List<string>>();
}