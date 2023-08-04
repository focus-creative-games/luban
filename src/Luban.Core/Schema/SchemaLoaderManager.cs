using System.Reflection;
using Luban.Utils;

namespace Luban.Schema;

public class SchemaLoaderManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    public static SchemaLoaderManager Ins { get; } = new ();

    private class LoaderInfo
    {
        public string Type { get; init; }
        
        public string[] ExtNames { get; init; }
        
        public int Priority { get; init; }
        
        public Func<ISchemaLoader> Creator { get; init; }
    }
    
    private readonly List<LoaderInfo> _schemaLoaders = new();

    public ISchemaLoader Create(string extName, string type, ISchemaCollector collector, object args)
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
        schemaLoader.Arguments = args;
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
}