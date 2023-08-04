using System.Reflection;
using System.Text;
using System.Text.Json;
using NLog;

namespace Luban.Core.Plugin;

public class PluginManager
{
    private static readonly ILogger s_logger = LogManager.GetCurrentClassLogger();
    public static PluginManager Ins { get; } = new PluginManager();
    
    private readonly List<IPlugin> _plugins = new List<IPlugin>();

    public IReadOnlyList<IPlugin> Plugins => _plugins;

    public void Init(IPluginCollector pluginCollector)
    {
        foreach (string pluginPath in pluginCollector.GetPluginPaths())
        {
            if (LoadPlugin(pluginPath, out var plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }

    private Assembly LoadPluginAssembly(string pluginDir, string entryDllName)
    {
        s_logger.Info($"load plugin:{pluginDir}/{entryDllName}");
        var loadContext = new PluginLoadContext($"{pluginDir}/{entryDllName}");
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(entryDllName)));
    }

    class PluginConfig
    {
        public string Name { get; set; }
        
        public bool Enabled { get; set; }
        
        public string EntryDll { get; set; }
    }
    
    private bool LoadPlugin(string pluginPath, out IPlugin plugin)
    {
        string jsonStr = File.ReadAllText($"{pluginPath}/plugin.json", Encoding.UTF8);
        var pluginConf = JsonSerializer.Deserialize<PluginConfig>(jsonStr);
        if (!pluginConf.Enabled)
        {
            s_logger.Info($"plugin:{pluginPath} is disabled");
            plugin = null;
            return false;
        }
        
        var ass = LoadPluginAssembly(pluginPath, pluginConf.EntryDll);
        if (ass == null)
        {
            throw new PluginLoadException($"plugin:{pluginPath} doesn't exists");
        }
        s_logger.Info($"plugin:{pluginPath} is enabled");
        foreach (var type in ass.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsAbstract)
            {
                plugin = (IPlugin)Activator.CreateInstance(type);
                plugin.Location = pluginPath;
                plugin.Init(jsonStr);
                return true;
            }
        }
        throw new PluginLoadException($"can't find any type derived from IPlugin in plugin:{pluginPath} ");
    }
    
}