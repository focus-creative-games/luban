using System.Reflection;
using Luban.CustomBehaviour;

namespace Luban.CodeTarget;

public class CodeTargetManager
{
    public static CodeTargetManager Ins { get; } = new();

    public void Init()
    {

    }

    public ICodeTarget CreateCodeTarget(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<ICodeTarget, CodeTargetAttribute>(name);
    }
}
