using Luban.CustomBehaviour;

namespace Luban.OutputSaver;

[AttributeUsage(AttributeTargets.Class)]
public class OutputSaverAttribute : BehaviourBaseAttribute
{
    public OutputSaverAttribute(string name) : base(name)
    {
    }
}
