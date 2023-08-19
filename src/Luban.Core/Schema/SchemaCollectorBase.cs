using Luban.RawDefs;

namespace Luban.Schema;

public abstract class SchemaCollectorBase : ISchemaCollector
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly List<RawEnum> _enums = new();
    private readonly List<RawBean> _beans = new();

    private readonly Dictionary<string, string> _envs = new();

    private readonly List<RawTable> _tables = new();

    private readonly List<RawTarget> _targets = new();

    private readonly List<RawGroup> _groups = new();

    private readonly List<RawRefGroup> _refGroups = new();

    protected List<RawTable> Tables => _tables;
    
    public abstract void Load(string schemaPath);
    
    public RawAssembly CreateRawAssembly()
    {
        return new RawAssembly()
        {
            Tables = _tables,
            Targets = _targets,
            Groups = _groups,
            RefGroups = _refGroups,
            Enums = _enums,
            Beans = _beans,
            Options = _envs,
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

    public void Add(RawGroup group)
    {
        lock (this)
        {
            _groups.Add(group);
        }
    }

    public void Add(RawRefGroup refGroup)
    {
        lock (this)
        {
            _refGroups.Add(refGroup);
        }
    }

    public void Add(RawTarget target)
    {
        lock (this)
        {
            _targets.Add(target);
        }
    }

    public void AddEnv(string key, string value)
    {
        lock (this)
        {
            if (!_envs.TryAdd(key, value))
            {
                s_logger.Warn("env key:{} already exist, value:{} will be ignored", key, value);
            }
        }
    }
}