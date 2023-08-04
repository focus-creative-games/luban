using Luban.Core.RawDefs;

namespace Luban.Core.Defs;

public class DefRefGroup
{
    public string Name { get; }

    public List<string> Refs { get; }

    public DefRefGroup(RawRefGroup group)
    {
        this.Name = group.Name;
        this.Refs = group.Refs;
    }
}