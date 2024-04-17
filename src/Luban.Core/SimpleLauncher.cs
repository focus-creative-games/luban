using System.Reflection;
using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.CustomBehaviour;
using Luban.DataLoader;
using Luban.DataTarget;
using Luban.L10N;
using Luban.OutputSaver;
using Luban.Pipeline;
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
        string dllDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        foreach (var dllFile in Directory.GetFiles(dllDir, "*.dll", SearchOption.TopDirectoryOnly))
        {
            string dllName = Path.GetFileNameWithoutExtension(dllFile);
            if (dllName.Contains("Luban") && AppDomain.CurrentDomain.GetAssemblies().All(a => a.GetName().Name != dllName))
            {
                s_logger.Trace("load dll:{dll}", dllFile);
                Assembly.Load(dllName);
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
}
