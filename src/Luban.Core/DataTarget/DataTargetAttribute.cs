using Luban.CustomBehaviour;

namespace Luban.DataTarget;

[AttributeUsage(AttributeTargets.Class)]
public class DataTargetAttribute : BehaviourBaseAttribute
{
    public DataTargetAttribute(string name) : base(name)
    {
    }
}
