namespace Luban.Plugin;

public interface IPluginCollector
{
    IReadOnlyList<string> GetPluginPaths();
}