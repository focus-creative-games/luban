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

    private readonly GenerationArguments _genArgs;
    
    private RawAssembly _rawAssembly;
    
    private DefAssembly _defAssembly;

    private GenerationContext _genCtx;

    public DefaultPipeline()
    {
        _genArgs = GenerationContext.CurrentArguments;
    }

    public void Run()
    {
        LoadSchema();
        PrepareGenerationContext();
        ProcessTargets();
    }

    protected void LoadSchema()
    {
        string schemaCollectorName = _genArgs.SchemaCollector;
        s_logger.Info("load schema. collector: {}  path:{}", schemaCollectorName, _genArgs.SchemaPath);
        var schemaCollector = SchemaManager.Ins.CreateSchemaCollector(schemaCollectorName);
        schemaCollector.Load(_genArgs.SchemaPath);
        _rawAssembly = schemaCollector.CreateRawAssembly();
    }

    protected void PrepareGenerationContext()
    {
        s_logger.Debug("prepare generation context");
        _defAssembly = new DefAssembly(_rawAssembly, _genArgs.Target, _genArgs.OutputTables);
        _genCtx = new GenerationContext(_defAssembly);
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
        foreach (string target in _genArgs.CodeTargets)
        {
            ICodeTarget m = CodeTargetManager.Ins.GetCodeTarget(target);
            tasks.Add(Task.Run(() => ProcessCodeTarget(target, m)));
        }

        if (_genArgs.DataTargets.Count > 0)
        {
            LoadDatas();
            string dataExporterName = _genCtx.GetOptionOrDefault("global", "dataExporter", true, "default");
            s_logger.Debug("dataExporter: {}", dataExporterName);
            IDataExporter dataExporter = DataTargetManager.Ins.GetDataExporter(dataExporterName);
            foreach (string mission in _genArgs.DataTargets)
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
        
        if (_genArgs.TryGetOption(name, "postprocess", true, out string postProcessName))
        {
            var oldManifest = outputManifest;
            outputManifest = new OutputFileManifest();
            PostProcessManager.Ins.GetPostProcess(postProcessName).PostProcess(oldManifest, outputManifest);
        }

        string outputSaverName = _genArgs.TryGetOption(name, "outputSaver", true, out string outputSaver)
            ? outputSaver
            : "local";
        var saver = OutputSaverManager.Ins.GetOutputSaver(outputSaverName);
        string outputDir = _genArgs.GetOption($"{CodeTargetBase.FamilyPrefix}.{name}", "outputCodeDir", true);
        saver.Save(outputManifest, outputDir);
        s_logger.Info("process code target:{} end", name);
    }
    
    protected void ProcessDataTarget(string name, IDataExporter mission, IDataTarget dataTarget)
    {
        s_logger.Info("process data target:{} begin", name);
        var outputManifest = new OutputFileManifest();
        mission.Handle(_genCtx, dataTarget, outputManifest);
        
        if (_genArgs.TryGetOption(name, "postprocess", true, out string postProcessName))
        {
            var oldManifest = outputManifest;
            outputManifest = new OutputFileManifest();
            PostProcessManager.Ins.GetPostProcess(postProcessName).PostProcess(oldManifest, outputManifest);
        }

        string outputSaverName = _genArgs.TryGetOption(name, "outputSaver", true, out string outputSaver)
            ? outputSaver
            : "local";
        var saver = OutputSaverManager.Ins.GetOutputSaver(outputSaverName);
        string outputDir = _genArgs.GetOption($"{DataExporterBase.FamilyPrefix}.{name}", "outputDataDir", true);
        saver.Save(outputManifest, outputDir);
        s_logger.Info("process data target:{} end", name);
    }
    
}