using Luban.RawDefs;

namespace Luban.Schema;

public class LubanConfig
{
    public string ConfigFileName { get; set; }
    public List<RawGroup> Groups { get; set; }

    public List<RawTarget> Targets { get; set; }

    public List<SchemaFileInfo> Imports { get; set; }

    public List<string> Xargs { get; set; }

    public string InputDataDir { get; set; }
}
