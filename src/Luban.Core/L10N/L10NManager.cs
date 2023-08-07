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
        ITextProvider provider = CustomBehaviourManager.Ins.CreateBehaviour<ITextProvider, TextProviderAttribute>(name);
        provider.Load();
        return provider;
    }
    
    public ITextProvider GetOrCreateContextUniqueTextProvider(string name)
    {
        return (ITextProvider)GenerationContext.Current.GetOrAddUniqueObject($"{BuiltinOptionNames.TextProviderName}.{name}", () => CreateTextProvider(name));
    }
}