using Luban.CodeTarget;
using Luban.DataTarget;
using Luban.Defs;
using Luban.OutputSaver;
using Luban.PostProcess;
using Luban.RawDefs;
using Luban.Schema;
using Luban.Validator;
using NLog;

namespace Luban.Pipeline;

[Pipeline("default")]
public class DefaultPipeline : IPipeline
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    private PipelineArguments _args;
    
    private RawAssembly _rawAssembly;
    
    private DefAssembly _defAssembly;

    private GenerationContext _genCtx;

    public DefaultPipeline()
    {
    }

    public void Run(PipelineArguments args)
    {
        _args = args;
        LoadSchema();
        PrepareGenerationContext();
        ProcessTargets();
    }

    protected void LoadSchema()
    {
        string schemaCollectorName = _args.SchemaCollector;
        s_logger.Info("load schema. collector: {}  path:{}", schemaCollectorName, _args.SchemaPath);
        var schemaCollector = SchemaManager.Ins.CreateSchemaCollector(schemaCollectorName);
        schemaCollector.Load(_args.SchemaPath);
        _rawAssembly = schemaCollector.CreateRawAssembly();
    }

    protected void PrepareGenerationContext()
    {
        s_logger.Debug("prepare generation context");
        _defAssembly = new DefAssembly(_rawAssembly, _args.Target, _args.OutputTables);
        
        var generationCtxBuilder = new GenerationContextBuilder
        {
            Assembly = _defAssembly,
            IncludeTags = _args.IncludeTags,
            ExcludeTags = _args.ExcludeTags,
            TimeZone = _args.TimeZone,
        };
        _genCtx = new GenerationContext(generationCtxBuilder);
    }

    protected void LoadDatas()
    {
        _genCtx.LoadDatas();
        DoValidate();
    }

    protected void DoValidate()
    {
        var v = new DataValidatorContext(_defAssembly);
        v.ValidateTables(_genCtx.Tables);
    }

    protected void ProcessTargets()
    {
        var tasks = new List<Task>();
        foreach (string target in _args.CodeTargets)
        {
            ICodeTarget m = CodeTargetManager.Ins.GetCodeTarget(target);
            tasks.Add(Task.Run(() => ProcessCodeTarget(target, m)));
        }

        if (_args.DataTargets.Count > 0)
        {
            LoadDatas();
            string dataExporterName = EnvManager.Current.GetOptionOrDefault("", BuiltinOptionNames.DataExporter, true, "default");
            s_logger.Debug("dataExporter: {}", dataExporterName);
            IDataExporter dataExporter = DataTargetManager.Ins.GetDataExporter(dataExporterName);
            foreach (string mission in _args.DataTargets)
            {
                IDataTarget dataTarget = DataTargetManager.Ins.GetTableExporter(mission);
                tasks.Add(Task.Run(() => ProcessDataTarget(mission, dataExporter, dataTarget)));
            }
        }
        Task.WaitAll(tasks.ToArray());
    }

    protected void ProcessCodeTarget(string name, ICodeTarget codeTarget)
    {
        s_logger.Info("process code target:{} begin", name);
        var outputManifest = new OutputFileManifest();
        codeTarget.Handle(_genCtx, outputManifest);
        
        if (EnvManager.Current.TryGetOption(name, BuiltinOptionNames.Postprocess, true, out string postProcessName))
        {
            var oldManifest = outputManifest;
            outputManifest = new OutputFileManifest();
            PostProcessManager.Ins.GetPostProcess(postProcessName).PostProcess(oldManifest, outputManifest);
        }

        string outputSaverName = EnvManager.Current.GetOptionOrDefault(name, BuiltinOptionNames.OutputSaver, true, "local");
        var saver = OutputSaverManager.Ins.GetOutputSaver(outputSaverName);
        string outputDir = EnvManager.Current.GetOption($"{CodeTargetBase.FamilyPrefix}.{name}", BuiltinOptionNames.OutputCodeDir, true);
        saver.Save(outputManifest, outputDir);
        s_logger.Info("process code target:{} end", name);
    }
    
    protected void ProcessDataTarget(string name, IDataExporter mission, IDataTarget dataTarget)
    {
        s_logger.Info("process data target:{} begin", name);
        var outputManifest = new OutputFileManifest();
        mission.Handle(_genCtx, dataTarget, outputManifest);
        
        if (EnvManager.Current.TryGetOption(name, BuiltinOptionNames.Postprocess, true, out string postProcessName))
        {
            var oldManifest = outputManifest;
            outputManifest = new OutputFileManifest();
            PostProcessManager.Ins.GetPostProcess(postProcessName).PostProcess(oldManifest, outputManifest);
        }

        string outputSaverName = EnvManager.Current.GetOptionOrDefault(name, BuiltinOptionNames.OutputSaver, true, "local");
        var saver = OutputSaverManager.Ins.GetOutputSaver(outputSaverName);
        string outputDir = EnvManager.Current.GetOption($"{DataExporterBase.FamilyPrefix}.{name}", BuiltinOptionNames.OutputDataDir, true);
        saver.Save(outputManifest, outputDir);
        s_logger.Info("process data target:{} end", name);
    }
    
}