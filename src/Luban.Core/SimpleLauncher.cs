using System.Reflection;
using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.DataLoader;
using Luban.DataTarget;
using Luban.OutputSaver;
using Luban.Plugin;
using Luban.PostProcess;
using Luban.Schema;
using Luban.Tmpl;

namespace Luban;

public class SimpleLauncher
{
    public void Start(List<Assembly> builtinAssemblies)
    {
        InitManagers();
        ScanRegisterBuiltinAssemblies(builtinAssemblies);
        ScanRegisterPlugins();
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
        DataTargetManager.Ins.Init();
        PluginManager.Ins.Init();
    }

    private void ScanRegisterBuiltinAssemblies(List<Assembly> builtinAssemblies)
    {
        foreach (var assembly in builtinAssemblies)
        {
            ScanRegisterAssembly(assembly);
        }
    }

    private void ScanRegisterAssembly(Assembly assembly)
    {
        SchemaManager.Ins.ScanRegisterAll(assembly);
        CodeFormatManager.Ins.ScanRegisterAll(assembly);
        CodeTargetManager.Ins.ScanResisterCodeTarget(assembly);
        PostProcessManager.Ins.ScanRegisterPostProcess(assembly);
        OutputSaverManager.Ins.ScanRegisterOutputSaver(assembly);
        DataLoaderManager.Ins.ScanRegisterDataLoader(assembly);
        DataTargetManager.Ins.ScanRegisterAll(assembly);
    }

    private void ScanRegisterPlugins()
    {
        foreach (var plugin in PluginManager.Ins.Plugins)
        {
            TemplateManager.Ins.AddTemplateSearchPath($"{plugin.Location}/Templates", false);
            ScanRegisterAssembly(plugin.GetType().Assembly);
        }
    }
}