using System.Reflection;

namespace Luban.Core.CodeTarget;

public class CodeTargetManager
{
    public static CodeTargetManager Ins { get; } = new();

    private readonly Dictionary<string, ICodeTarget> _codeTargets = new();
    
    public void Init()
    {
        
    }

    public ICodeTarget GetCodeTarget(string name)
    {
        return _codeTargets.TryGetValue(name, out var codeTarget)
            ? codeTarget
            : throw new Exception($"code target:{name} not exists");
    }
    
    public void Register(string name, ICodeTarget codeTarget)
    {
        if (!_codeTargets.TryAdd(name, codeTarget))
        {
            throw new Exception($"code target:{name} already exists");
        }
    }

    public void ScanResisterCodeTarget(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            if (type.GetCustomAttribute<CodeTargetAttribute>() is { } attr)
            {
                if (!typeof(ICodeTarget).IsAssignableFrom(type))
                {
                    throw new Exception($"type:{type.FullName} not implement interface:{typeof(ICodeTarget).FullName}");
                }

                var codeTarget = Activator.CreateInstance(type) as ICodeTarget;
                Register(attr.Name, codeTarget);
            }
        }
    }
}