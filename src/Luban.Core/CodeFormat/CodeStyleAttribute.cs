using Luban.CustomBehaviour;

namespace Luban.CodeFormat;

[AttributeUsage(AttributeTargets.Class)]
public class CodeStyleAttribute : BehaviourBaseAttribute
{
    public CodeStyleAttribute(string name) : base(name)
    {
    }
}
