using System.Reflection;

namespace Luban.Core.DataTarget;

public class DataTargetManager
{
    public static DataTargetManager Ins { get; } = new();

    private readonly Dictionary<string, IDataExporter> _dataExporters = new();
    private readonly Dictionary<string, IDataTarget> _tableExporters = new();

    public void Init()
    {
        
    }
    
    public IDataExporter GetDataExporter(string name)
    {
        if (_dataExporters.TryGetValue(name, out var exporter))
        {
            return exporter;
        }
        throw new Exception($"not found data exporter:{name}");
    }
    
    public void RegisterDataExporter(string name, IDataExporter exporter)
    {
        if (_dataExporters.ContainsKey(name))
        {
            throw new Exception($"data exporter:{name} is already registered");
        }
        _dataExporters.Add(name, exporter);
    }
    
    public void ScanRegisterDataExporter(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }
            if (t.GetCustomAttribute<DataExporterAttribute>() is { } attr)
            {
                var exporter = Activator.CreateInstance(t) as IDataExporter;
                RegisterDataExporter(attr.Name, exporter);
            }
        }
    }
    
    public IDataTarget GetTableExporter(string name)
    {
        if (_tableExporters.TryGetValue(name, out var exporter))
        {
            return exporter;
        }
        throw new Exception($"not found table exporter:{name}");
    }
    
    public void RegisterTableExporter(string name, IDataTarget exporter)
    {
        if (_tableExporters.ContainsKey(name))
        {
            throw new Exception($"table exporter:{name} is already registered");
        }
        _tableExporters.Add(name, exporter);
    }
    
    public void ScanRegisterTableExporter(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }
            if (t.GetCustomAttribute<DataTargetAttribute>() is { } attr)
            {
                var exporter = Activator.CreateInstance(t) as IDataTarget;
                RegisterTableExporter(attr.Name, exporter);
            }
        }
    }
}