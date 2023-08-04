namespace Luban.Plugin;

public abstract class PluginBase : IPlugin
{
    public abstract string Name { get; }
    
    public string Location { get; set; }
    
    public abstract void Init(string jsonStr);
    
    public abstract void Start();
}