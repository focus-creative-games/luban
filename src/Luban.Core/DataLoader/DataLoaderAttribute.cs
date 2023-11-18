using Luban.CustomBehaviour;

namespace Luban.DataLoader;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DataLoaderAttribute : BehaviourBaseAttribute
{
    public DataLoaderAttribute(string name) : base(name)
    {
    }
}
