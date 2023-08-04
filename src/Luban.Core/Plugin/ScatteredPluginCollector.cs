namespace Luban.Core.Plugin;

public class ScatteredPluginCollector : IPluginCollector
{
    private readonly string[] _pluginPaths;
    
    public ScatteredPluginCollector(params string[] pluginPaths)
    {
        _pluginPaths = pluginPaths;
    }
    
    public IReadOnlyList<string> GetPluginPaths()
    {
        return _pluginPaths;
    }
}