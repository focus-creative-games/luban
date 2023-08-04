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
        
        public Func<string, ISchemaLoader> Creator { get; init; }
    }
    
    private readonly List<LoaderInfo> _schemaLoaders = new();

    public ISchemaLoader Create(string extName, string type)
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

        return loader.Creator(type);
    }
    
    public void RegisterSchemaLoaderCreator(string type, string[] extNames, int priority, Func<string, ISchemaLoader> creator)
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
                var attr = t.GetCustomAttribute<SchemaLoaderAttribute>();
                var creator = (Func<string, ISchemaLoader>)Delegate.CreateDelegate(typeof(Func<string, ISchemaLoader>), t, "Create");
                RegisterSchemaLoaderCreator(attr.Type, attr.ExtNames, attr.Priority, creator);
            }
        }
    }
}