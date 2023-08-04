namespace Luban.Plugin;

public class DefaultPluginCollector : IPluginCollector
{
    private readonly List<string> _pluginPaths = new List<string>();
    
    public DefaultPluginCollector(params string[] rootDirs)
    {
        foreach (string rootDir in rootDirs)
        {
            string[] candidateDirs = Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories);
            foreach (string candidateDir in candidateDirs)
            {
                string pluginConfFile = $"{candidateDir}/plugin.json";
                if (!File.Exists(pluginConfFile))
                {
                    continue;
                }
                _pluginPaths.Add(candidateDir);
            }
        }    
    }
    
    public IReadOnlyList<string> GetPluginPaths()
    {
        return _pluginPaths;
    }
}