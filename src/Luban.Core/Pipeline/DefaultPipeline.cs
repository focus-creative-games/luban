using Luban.CodeTarget;
using Luban.DataTarget;
using Luban.Defs;
using Luban.L10N;
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

    private LubanConfig _config;

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
        _config = args.Config;
        LoadSchema();
        PrepareGenerationContext();
        ProcessTargets();
    }

    protected void LoadSchema()
    {
        string schemaCollectorName = _args.SchemaCollector;
        s_logger.Info("load schema. collector: {}", schemaCollectorName);
        var schemaCollector = SchemaManager.Ins.CreateSchemaCollector(schemaCollectorName);
        schemaCollector.Load(_config);
        _rawAssembly = schemaCollector.CreateRawAssembly();
    }

    protected void PrepareGenerationContext()
    {
        s_logger.Debug("prepare generation context");
        _genCtx = new GenerationContext();
        _defAssembly = new DefAssembly(_rawAssembly, _args.Target, _args.OutputTables, _config.Groups, _args.Variants);

        var generationCtxBuilder = new GenerationContextBuilder
        {
            Assembly = _defAssembly,
            IncludeTags = _args.IncludeTags,
            ExcludeTags = _args.ExcludeTags,
            TimeZone = _args.TimeZone,
        };
        _genCtx.Init(generationCtxBuilder);
    }

    protected void LoadDatas()
    {
        _genCtx.LoadDatas();
        DoValidate();
        ProcessL10N();
    }

    protected void DoValidate()
    {
        s_logger.Info("validation begin");
        var v = new DataValidatorContext(_defAssembly);
        v.ValidateTables(_genCtx.Tables);
        s_logger.Info("validation end");
    }

    protected void ProcessL10N()
    {
        if (_genCtx.TextProvider != null)
        {
            _genCtx.TextProvider.ProcessDatas();
        }
    }

    protected void ProcessTargets()
    {
        var tasks = new List<Task>();
        tasks.Add(Task.Run(() =>
        {
            foreach (string target in _args.CodeTargets)
            {
                // code target doesn't support run in parallel
                ICodeTarget m = CodeTargetManager.Ins.CreateCodeTarget(target);
                ProcessCodeTarget(target, m);
            }
        }));

        if (_args.ForceLoadTableDatas || _args.DataTargets.Count > 0)
        {
            LoadDatas();
        }

        if (_args.DataTargets.Count > 0)
        {
            string dataExporterName = EnvManager.Current.GetOptionOrDefault("", BuiltinOptionNames.DataExporter, true, "default");
            s_logger.Debug("dataExporter: {}", dataExporterName);
            IDataExporter dataExporter = DataTargetManager.Ins.CreateDataExporter(dataExporterName);
            foreach (string mission in _args.DataTargets)
            {
                IDataTarget dataTarget = DataTargetManager.Ins.CreateDataTarget(mission);
                tasks.Add(Task.Run(() => ProcessDataTarget(mission, dataExporter, dataTarget)));
            }
        }
        Task.WaitAll(tasks.ToArray());
    }

    protected void ProcessCodeTarget(string name, ICodeTarget codeTarget)
    {
        s_logger.Info("process code target:{} begin", name);
        var outputManifest = new OutputFileManifest(name, OutputType.Code);
        GenerationContext.CurrentCodeTarget = codeTarget;
        codeTarget.ValidateDefinition(_genCtx);
        codeTarget.Handle(_genCtx, outputManifest);

        outputManifest = PostProcess(BuiltinOptionNames.CodePostprocess, outputManifest);
        Save(outputManifest);
        s_logger.Info("process code target:{} end", name);
    }

    protected OutputFileManifest PostProcess(string familyName, OutputFileManifest manifest)
    {
        string name = manifest.TargetName;
        if (EnvManager.Current.TryGetOption(name, familyName, true, out string postProcessName))
        {
            var newManifest = new OutputFileManifest(name, manifest.OutputType);
            PostProcessManager.Ins.GetPostProcess(postProcessName).PostProcess(manifest, newManifest);
            return newManifest;
        }
        return manifest;
    }

    protected void ProcessDataTarget(string name, IDataExporter mission, IDataTarget dataTarget)
    {
        s_logger.Info("process data target:{} begin", name);
        var outputManifest = new OutputFileManifest(name, OutputType.Data);
        mission.Handle(_genCtx, dataTarget, outputManifest);

        var newManifest = PostProcess(BuiltinOptionNames.DataPostprocess, outputManifest);
        Save(newManifest);
        s_logger.Info("process data target:{} end", name);
    }

    private void Save(OutputFileManifest manifest)
    {
        string name = manifest.TargetName;
        string outputSaverName = EnvManager.Current.GetOptionOrDefault(name, BuiltinOptionNames.OutputSaver, true, "local");
        var saver = OutputSaverManager.Ins.GetOutputSaver(outputSaverName);
        saver.Save(manifest);
    }

}
