namespace Luban.Plugin;

public class PluginLoadException : Exception
{
    public PluginLoadException(string message) : base(message)
    {
    }
}