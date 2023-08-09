using System.Reflection;
using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.CustomBehaviour;
using Luban.DataLoader;
using Luban.DataTarget;
using Luban.L10N;
using Luban.OutputSaver;
using Luban.Pipeline;
using Luban.Plugin;
using Luban.PostProcess;
using Luban.Schema;
using Luban.Tmpl;
using Luban.Validator;

namespace Luban;

public class SimpleLauncher
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    public void Start(Dictionary<string, string> options)
    {
        EnvManager.Current = new EnvManager(options);
        InitManagers();
        ScanRegisterAssemblyBehaviours();
        ScanRegisterPlugins();
        PostInitManagers();
    }
    
    private void InitManagers()
    {
        SchemaManager.Ins.Init();
        TemplateManager.Ins.Init();
        CodeFormatManager.Ins.Init();
        CodeTargetManager.Ins.Init();
        PostProcessManager.Ins.Init();
        OutputSaverManager.Ins.Init();
        DataLoaderManager.Ins.Init();
        ValidatorManager.Ins.Init();
        DataTargetManager.Ins.Init();
        PluginManager.Ins.Init();
        PipelineManager.Ins.Init();
        L10NManager.Ins.Init();
        CustomBehaviourManager.Ins.Init();
    }

    private void PostInitManagers()
    {
        CodeFormatManager.Ins.PostInit();
    }

    private void ScanRegisterAssemblyBehaviours()
    {
        string dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var loadedAssemblyNames = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).ToHashSet();
        foreach (var dllFile in Directory.GetFiles(dllDir, "*.dll", SearchOption.TopDirectoryOnly))
        {
            string dllName = Path.GetFileNameWithoutExtension(dllFile);
            if (!loadedAssemblyNames.Contains(dllName))
            {
                s_logger.Info("load dll:{dll}", dllFile);
                Assembly.LoadFile(dllFile);
            }
        }
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetCustomAttribute<RegisterBehaviourAttribute>() != null)
            {
                ScanRegisterAssembly(assembly);
            }
        }
    }

    public void ScanRegisterAssembly(Assembly assembly)
    {
        CustomBehaviourManager.Ins.ScanRegisterBehaviour(assembly);
        SchemaManager.Ins.ScanRegisterAll(assembly);
    }

    private void ScanRegisterPlugins()
    {
        foreach (var plugin in PluginManager.Ins.Plugins)
        {
            TemplateManager.Ins.AddTemplateSearchPath($"{plugin.Location}/Templates", false);
        }
    }
}