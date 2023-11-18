using System.Reflection;
using Luban.CustomBehaviour;

namespace Luban.Pipeline;

public class PipelineManager
{
    public static PipelineManager Ins { get; } = new();

    public void Init()
    {

    }

    public IPipeline CreatePipeline(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IPipeline, PipelineAttribute>(name);
    }
}
