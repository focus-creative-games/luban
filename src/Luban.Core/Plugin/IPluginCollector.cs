namespace Luban.Core.Plugin;

public interface IPluginCollector
{
    IReadOnlyList<string> GetPluginPaths();
}