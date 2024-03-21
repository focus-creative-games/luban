using Luban.CustomBehaviour;

namespace Luban.L10N;

public class L10NManager
{
    public static L10NManager Ins { get; } = new();

    public void Init()
    {

    }

    public ITextProvider CreateTextProvider(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<ITextProvider, TextProviderAttribute>(name);
    }
}
