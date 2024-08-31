using System.Reflection;

namespace Luban.CustomBehaviour;

public class CustomBehaviourManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static CustomBehaviourManager Ins { get; } = new();

    private class BehaviourInfo
    {
        public int Priority { get; set; }

        public Func<object> Creator { get; set; }
    }

    private readonly Dictionary<(Type, string), BehaviourInfo> _behaviourCreators = new();

    public void Init()
    {
        _behaviourCreators.Clear();
    }

    public T CreateBehaviour<T, C>(string name) where C : Attribute, ICustomBehaviour where T : class
    {
        if (_behaviourCreators.TryGetValue((typeof(C), name), out var bi))
        {
            return (T)bi.Creator();
        }

        throw new Exception($"behaviour:{name} type:{typeof(T)} not exists");
    }

    public bool TryCreateBehaviour<T, C>(string name, out T behaviour) where C : Attribute, ICustomBehaviour where T : class
    {
        if (_behaviourCreators.TryGetValue((typeof(C), name), out var bi))
        {
            behaviour = (T)bi.Creator();
            return true;
        }

        behaviour = null;
        return false;
    }

    public void RegisterBehaviour(Type type, string name, int priority, Func<object> behaviourCreator)
    {
        if (_behaviourCreators.TryGetValue((type, name), out var bi))
        {
            if (bi.Priority >= priority)
            {
                s_logger.Warn("Behaviour type:{} name:{} priority:{} is ignored", type, name, priority);
                return;
            }
            s_logger.Debug("Behaviour type:{} name:{} priority:{} is overrided by priority:{}", type, name, bi.Priority, priority);
        }
        s_logger.Trace("register behaviour type:{} name:{} priority:{}", type, name, priority);
        _behaviourCreators[(type, name)] = new BehaviourInfo() { Priority = priority, Creator = behaviourCreator };
    }

    public void ScanRegisterBehaviour(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }
            foreach (var attr in t.GetCustomAttributes<BehaviourBaseAttribute>())
            {
                RegisterBehaviour(attr.GetType(), attr.Name, attr.Priority, () => Activator.CreateInstance(t));
            }
        }
    }
}
