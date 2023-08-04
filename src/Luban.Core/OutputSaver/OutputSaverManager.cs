using System.Reflection;

namespace Luban.Core.OutputSaver;

public class OutputSaverManager
{
    public static OutputSaverManager Ins { get; } = new OutputSaverManager();
    
    private readonly Dictionary<string, IOutputSaver> _outputSavers = new ();

    public void Init()
    {
        ScanRegisterOutputSaver(GetType().Assembly);
    }
    
    public IOutputSaver GetOutputSaver(string name)
    {
        if (_outputSavers.TryGetValue(name, out var outputSaver))
        {
            return outputSaver;
        }
        else
        {
            throw new Exception($"output saver:{name} not found");
        }
    }
    
    public void RegisterOutputSaver(string name, IOutputSaver outputSaver)
    {
        if (!_outputSavers.TryAdd(name, outputSaver))
        {
            throw new Exception($"output saver:{name} duplicate");
        }
    }
    
    public void ScanRegisterOutputSaver(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }

            if (typeof(IOutputSaver).IsAssignableFrom(t))
            {
                var attr = t.GetCustomAttribute<OutputSaverAttribute>();
                if (attr == null)
                {
                    throw new Exception($"output saver:{t.FullName} must has attribute:{nameof(OutputSaverAttribute)}");
                }

                var outputSaver = (IOutputSaver)Activator.CreateInstance(t);
                RegisterOutputSaver(attr.Name, outputSaver);
            }
        }
    }
    
}