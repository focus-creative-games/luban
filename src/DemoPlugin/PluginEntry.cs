using Luban.Plugin;
using NLog;

namespace DemoPlugin;

public class PluginEntry : PluginBase
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
    
    public override string Name => "DemoPlugin";
    
    public override void Init(string jsonStr)
    {
        s_logger.Info($"plugin [{Name}] inits success");
    }

    public override void Start()
    {

    }
}