using System.Reflection;

namespace Luban.Pipeline;

public class PipelineManager
{
    public static PipelineManager Ins { get; } = new();

    private readonly Dictionary<string, Func<IPipeline>> _pipelines = new();

    public void Init()
    {
        ScanRegisterPipeline(GetType().Assembly);
    }
    
    public IPipeline CreatePipeline(string name)
    {
        if (_pipelines.TryGetValue(name, out var pipeline))
        {
            return pipeline();
        }
        throw new Exception($"not found pipeline:{name}");
    }
    
    public void RegisterPipeline(string name, Func<IPipeline> pipeline)
    {
        if (_pipelines.ContainsKey(name))
        {
            throw new Exception($"pipeline:{name} is already registered");
        }
        _pipelines.Add(name, pipeline);
    }
    
    public void ScanRegisterPipeline(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }
            if (t.GetCustomAttribute<PipelineAttribute>() is { } attr)
            {
                var pipeline = Activator.CreateInstance(t) as IPipeline;
                RegisterPipeline(attr.Name, () => pipeline);
            }
        }
    }
}