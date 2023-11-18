using Luban.CustomBehaviour;

namespace Luban.Pipeline;

[AttributeUsage(AttributeTargets.Class)]
public class PipelineAttribute : BehaviourBaseAttribute
{
    public PipelineAttribute(string name) : base(name)
    {

    }
}
