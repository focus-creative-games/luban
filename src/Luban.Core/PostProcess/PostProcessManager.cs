using System.Reflection;

namespace Luban.Core.PostProcess;

public class PostProcessManager
{
    public static PostProcessManager Ins { get; } = new PostProcessManager();
    
    private readonly Dictionary<string, IPostProcess> _postProcesses = new();

    public void Init()
    {
        ScanRegisterPostProcess(GetType().Assembly);
    }
    
    public IPostProcess GetPostProcess(string name)
    {
        if (_postProcesses.TryGetValue(name, out var postProcess))
        {
            return postProcess;
        }
        else
        {
            throw new Exception($"post process:{name} not found");
        }
    }
    
    public void RegisterPostProcess(string name, IPostProcess postProcess)
    {
        if (!_postProcesses.TryAdd(name, postProcess))
        {
            throw new Exception($"post process:{name} duplicate");
        }
    }

    public void ScanRegisterPostProcess(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }

            if (typeof(IPostProcess).IsAssignableFrom(t))
            {
                var attr = t.GetCustomAttribute<PostProcessAttribute>();
                if (attr == null)
                {
                    throw new Exception($"post process:{t.FullName} must has attribute:{nameof(PostProcessAttribute)}");
                }

                var postProcess = (IPostProcess)Activator.CreateInstance(t);
                RegisterPostProcess(attr.Name, postProcess);
            }
        }
    }
    
}