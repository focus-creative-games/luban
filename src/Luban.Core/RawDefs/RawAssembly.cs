namespace Luban.RawDefs;

public class RawAssembly
{
    public List<RawBean> Beans { get; set; } = new();

    public List<RawEnum> Enums { get; set; } = new();

    public List<RawTable> Tables { get; set; } = new();

    public List<RawGroup> Groups { get; set; } = new();

    public List<RawTarget> Targets { get; set; } = new();

    public List<RawRefGroup> RefGroups { get; set; } = new();
}
