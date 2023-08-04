using System.Reflection;

namespace Luban.Core.Schema;

public class SchemaCollectorManager
{
    public static SchemaCollectorManager Ins { get; } = new ();
    
    private readonly Dictionary<string, Func<ISchemaCollector>> _collectors = new();

    public void Init()
    {
        
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
}