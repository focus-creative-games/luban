using Luban.CustomBehaviour;

namespace Luban.CodeTarget;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CodeTargetAttribute : BehaviourBaseAttribute
{
    public CodeTargetAttribute(string name) : base(name)
    {
    }
}
