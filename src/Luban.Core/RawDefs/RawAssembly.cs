namespace Luban.RawDefs;

public class RawAssembly
{
    public Dictionary<string, string> Envs { get; set; } = new();

    public HashSet<string> ExternalSelectors { get; set; } = new();

    public List<RawExternalType> ExternalTypes { get; set; } = new();

    public List<RawBean> Beans { get; set; } = new();

    public List<RawEnum> Enums { get; set; } = new();

    public List<RawTable> Tables { get; set; } = new();

    public List<RawGroup> Groups { get; set; } = new();

    public List<RawTarget> Targets { get; set; } = new();

    public List<RawRefGroup> RefGroups { get; set; } = new();
}