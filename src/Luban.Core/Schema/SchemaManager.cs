using System.Reflection;
using Luban.Utils;

namespace Luban.Schema;

public class SchemaManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    public static SchemaManager Ins { get; } = new ();
    
    private readonly Dictionary<string, Func<ISchemaCollector>> _collectors = new();
    
    private class LoaderInfo
    {
        public string Type { get; init; }
        
        public string[] ExtNames { get; init; }
        
        public int Priority { get; init; }
        
        public Func<ISchemaLoader> Creator { get; init; }
    }
    
    private readonly List<LoaderInfo> _schemaLoaders = new();

    private readonly Dictionary<string, Func<IBeanSchemaLoader>> _beanSchemaLoaders = new();

    public void Init()
    {
        
    }
    
    public void ScanRegisterAll(Assembly assembly)
    {
        ScanRegisterCollectorCreator(assembly);
        ScanRegisterSchemaLoaderCreator(assembly);
        ScanRegisterBeanSchemaLoaderCreator(assembly);
    }
    
    public ISchemaCollector CreateSchemaCollector(string name)
    {
        if (_collectors.TryGetValue(name, out var creator))
        {
            return creator();
        }
        throw new Exception($"can't find schema collector:{name}");
    }
    
    public void RegisterCollectorCreator(string name, Func<ISchemaCollector> creator)
    {
        if (!_collectors.TryAdd(name, creator))
        {
            throw new Exception($"duplicate register schema collector:{name}");
        }
    }

    public void ScanRegisterCollectorCreator(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsDefined(typeof(SchemaCollectorAttribute), false))
            {
                var attr = type.GetCustomAttribute<SchemaCollectorAttribute>();
                RegisterCollectorCreator(attr.Name, () => (ISchemaCollector)Activator.CreateInstance(type));
            }
        }
    }

    public ISchemaLoader CreateSchemaLoader(string extName, string type, ISchemaCollector collector)
    {
        LoaderInfo loader = null;
        
        foreach (var l in _schemaLoaders)
        {
            if (l.Type == type && l.ExtNames.Contains(extName))
            {
                if (loader == null || loader.Priority < l.Priority)
                {
                    loader = l;
                }
            }
        }

        if (loader == null)
        {
            throw new Exception($"can't find schema loader for type:{type} extName:{extName}");
        }

        ISchemaLoader schemaLoader = loader.Creator();
        schemaLoader.Type = type;
        schemaLoader.Collector = collector;
        return schemaLoader;
    }
    
    public void RegisterSchemaLoaderCreator(string type, string[] extNames, int priority, Func<ISchemaLoader> creator)
    {
        _schemaLoaders.Add(new LoaderInfo(){ Type = type, ExtNames = extNames, Priority = priority, Creator = creator});
        s_logger.Debug("add schema loader creator. type:{} priority:{} extNames:{}", type, priority, StringUtil.CollectionToString(extNames));
    }
    
    public void ScanRegisterSchemaLoaderCreator(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsDefined(typeof(SchemaLoaderAttribute), false))
            {
                foreach (var attr in t.GetCustomAttributes<SchemaLoaderAttribute>())
                {
                    var creator = () => (ISchemaLoader)Activator.CreateInstance(t);
                    RegisterSchemaLoaderCreator(attr.Type, attr.ExtNames, attr.Priority, creator);
                }
            }
        }
    }
    
    public IBeanSchemaLoader CreateBeanSchemaLoader(string type)
    {
        if (_beanSchemaLoaders.TryGetValue(type, out var creator))
        {
            return creator();
        }
        else
        {
            throw new Exception($"can't find bean schema loader for type:{type}");
        }
    }
    
    public void RegisterBeanSchemaLoaderCreator(string type, Func<IBeanSchemaLoader> creator)
    {
        _beanSchemaLoaders.Add(type, creator);
        s_logger.Debug("add bean schema loader creator. type:{}", type);
    }
    
    public void ScanRegisterBeanSchemaLoaderCreator(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            var attr = t.GetCustomAttribute<BeanSchemaLoaderAttribute>();
            if (attr != null)
            {
                var creator = () => (IBeanSchemaLoader)Activator.CreateInstance(t);
                RegisterBeanSchemaLoaderCreator(attr.Name, creator);
            }
        }
    }
}