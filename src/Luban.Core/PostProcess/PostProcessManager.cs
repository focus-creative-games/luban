using Luban.CustomBehaviour;

namespace Luban.PostProcess;

public class PostProcessManager
{
    public static PostProcessManager Ins { get; } = new();

    public void Init()
    {

    }

    public IPostProcess GetPostProcess(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IPostProcess, PostProcessAttribute>(name);
    }
}
