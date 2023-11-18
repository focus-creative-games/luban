using Luban.RawDefs;

namespace Luban.Schema;

public abstract class SchemaCollectorBase : ISchemaCollector
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly List<RawEnum> _enums = new();
    private readonly List<RawBean> _beans = new();

    private readonly List<RawTable> _tables = new();

    private readonly List<RawRefGroup> _refGroups = new();

    protected List<RawTable> Tables => _tables;

    public abstract void Load(LubanConfig config);

    public abstract RawAssembly CreateRawAssembly();

    protected RawAssembly CreateRawAssembly(LubanConfig config)
    {
        return new RawAssembly()
        {
            Tables = _tables,
            Targets = config.Targets.ToList(),
            Groups = config.Groups.ToList(),
            RefGroups = _refGroups,
            Enums = _enums,
            Beans = _beans,
        };
    }

    public void Add(RawTable table)
    {
        lock (this)
        {
            _tables.Add(table);
        }
    }

    public void Add(RawBean bean)
    {
        lock (this)
        {
            _beans.Add(bean);
        }
    }

    public void Add(RawEnum @enum)
    {
        lock (this)
        {
            _enums.Add(@enum);
        }
    }

    public void Add(RawRefGroup refGroup)
    {
        lock (this)
        {
            _refGroups.Add(refGroup);
        }
    }
}
