using Luban.CustomBehaviour;

namespace Luban.CodeFormat;

[AttributeUsage(AttributeTargets.Class)]
public class NamingConventionAttribute : BehaviourBaseAttribute
{
    public NamingConventionAttribute(string name) : base(name)
    {
    }
}
