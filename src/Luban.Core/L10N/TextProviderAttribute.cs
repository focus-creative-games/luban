using Luban.CustomBehaviour;

namespace Luban.L10N;

[AttributeUsage(AttributeTargets.Class)]
public class TextProviderAttribute : BehaviourBaseAttribute
{
    public TextProviderAttribute(string name) : base(name)
    {
    }
}
