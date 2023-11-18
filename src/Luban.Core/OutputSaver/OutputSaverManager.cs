using System.Reflection;
using Luban.CustomBehaviour;

namespace Luban.OutputSaver;

public class OutputSaverManager
{
    public static OutputSaverManager Ins { get; } = new();

    public void Init()
    {

    }

    public IOutputSaver GetOutputSaver(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IOutputSaver, OutputSaverAttribute>(name);
    }
}
