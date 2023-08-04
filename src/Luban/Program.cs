using CommandLine;
using Luban.CodeTarget.CSharp;
using Luban.Core;
using Luban.Core.CodeFormat;
using Luban.Core.CodeTarget;
using Luban.Core.DataLoader;
using Luban.Core.DataTarget;
using Luban.Core.Defs;
using Luban.Core.OutputSaver;
using Luban.Core.Plugin;
using Luban.Core.PostProcess;
using Luban.Core.RawDefs;
using Luban.Core.Schema;
using Luban.Core.Tmpl;
using Luban.DataExporter.Builtin;
using Luban.DataLoader.Builtin;
using Luban.Schema.Default;
using Luban.Utils;
using NLog;
using System.Reflection;
using System.Text;

namespace Luban;

internal static class Program
{

    private class CommandOptions
    {
        [Option('t', "target", Required = true, HelpText = "target name")]
        public string Target { get; set; }
        
        [Option('c', "codeTarget", Required = false, HelpText = "code target name")]
        public IEnumerable<string> CodeTargets { get; set; }
        
        [Option('d', "dataTarget", Required = false, HelpText = "data target name")]
        public IEnumerable<string> DataTargets { get; set; }
        
        [Option('e', "excludeTag", Required = false, HelpText = "exclude tag")]
        public IEnumerable<string> ExcludeTags { get; set; }
        
        [Option('s', "schemaCollector", Required = false, HelpText = "schema collector name")]
        public string SchemaCollector { get; set; } = "default";
        
        [Option("schemaPath", Required = true, HelpText = "schema path")]
        public string SchemaPath { get; set; }
        
        [Option("tables", Required = false, HelpText = "tables")]
        public IEnumerable<string> Tables { get; set; }
        
        [Option("includedTables", Required = false, HelpText = "included tables")]
        public IEnumerable<string> IncludedTables { get; set; }
        
        [Option("excludedTables", Required = false, HelpText = "excluded tables")]
        public IEnumerable<string> ExcludedTables { get; set; }
        
        [Option('x', "xargs", Required = false, HelpText = "args like -x a=1 -x b=2")]
        public IEnumerable<string> Xargs { get; set; }
    }
    
    private static ILogger s_logger;

    private static GenerationArguments ParseArgs(string[] args)
    {
        var helpWriter = new StringWriter();
        var parser = new Parser(settings =>
        {
            settings.AllowMultiInstance = true;
            settings.HelpWriter = helpWriter;
        });

        var result = parser.ParseArguments<CommandOptions>(args);
        if (result.Tag == ParserResultType.NotParsed)
        {
            Console.Error.WriteLine(helpWriter.ToString());
            Environment.Exit(1);
        }
        var opts = ((Parsed<CommandOptions>)result).Value;

        return new GenerationArguments()
        {
            Target = opts.Target,
            CodeTargets = opts.CodeTargets?.ToList() ?? new List<string>(),
            DataTargets = opts.DataTargets?.ToList() ?? new List<string>(),
            ExcludeTags = opts.ExcludeTags?.ToList() ?? new List<string>(),
            SchemaCollector = opts.SchemaCollector,
            SchemaPath = opts.SchemaPath,
            Tables = opts.Tables?.ToList() ?? new List<string>(),
            IncludedTables = opts.IncludedTables?.ToList() ?? new List<string>(),
            GeneralArgs = opts.Xargs.Select(s =>
            {
                string[] pair = s.Split('=', 2);
                return (pair[0], pair[1]);
            }).ToDictionary(p => p.Item1, p => p.Item2),
        };
    }

    private static void Main(string[] args)
    {
        ConsoleUtil.EnableQuickEditMode(false);
        Console.OutputEncoding = Encoding.UTF8;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        InitSimpleNLogConfigure(LogLevel.Info);
        s_logger = LogManager.GetCurrentClassLogger();
        s_logger.Info("init logger success");


        var genArgs = ParseArgs(args);

        string curDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        TemplateManager templateManager = TemplateManager.Ins;
        templateManager.Init();
        templateManager.AddTemplateSearchPath($"{curDir}/Templates", true);
        
        CodeFormatManager.Ins.Init();
        CodeTargetManager.Ins.Init();
        PostProcessManager.Ins.Init();
        OutputSaverManager.Ins.Init();
        DataLoaderManager.Ins.Init();
        DataTargetManager.Ins.Init();
        
        PluginManager.Ins.Init(new DefaultPluginCollector($"{curDir}/Plugins"));
        
        var scanAssemblies = new List<Assembly>()
        {
            typeof(CsharpBinCodeTarget).Assembly,
            typeof(DefaultSchemaCollector).Assembly,
            typeof(FieldNames).Assembly,
            typeof(DefaultDataExporter).Assembly,
        };

        foreach (var assembly in scanAssemblies)
        {
            ScanRegisterAssembly(assembly);
        }

        foreach (var plugin in PluginManager.Ins.Plugins)
        {
            templateManager.AddTemplateSearchPath($"{plugin.Location}/Templates", false);
            ScanRegisterAssembly(plugin.GetType().Assembly);
        }

        int processorCount = Environment.ProcessorCount;
        ThreadPool.SetMinThreads(Math.Max(4, processorCount), 5);
        ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 4), 10);
        
        s_logger.Info("start");

        var pipeline = new Pipeline(genArgs);
        pipeline.Run();
        
        s_logger.Info("bye~");
    }

    private static void ScanRegisterAssembly(Assembly assembly)
    {
        SchemaCollectorFactory.Ins.ScanRegisterCollectorCreator(assembly);
        SchemaLoaderFactory.Ins.ScanRegisterSchemaLoaderCreator(assembly);
        CodeFormatManager.Ins.ScanRegisterFormatters(assembly);
        CodeFormatManager.Ins.ScanRegisterCodeStyle(assembly);
        CodeTargetManager.Ins.ScanResisterCodeTarget(assembly);
        PostProcessManager.Ins.ScanRegisterPostProcess(assembly);
        OutputSaverManager.Ins.ScanRegisterOutputSaver(assembly);
        DataLoaderManager.Ins.ScanRegisterDataLoader(assembly);
        DataTargetManager.Ins.ScanRegisterDataExporter(assembly);
        DataTargetManager.Ins.ScanRegisterTableExporter(assembly);
    }
    
    private static void InitSimpleNLogConfigure(NLog.LogLevel minConsoleLogLevel)
    {
        var logConfig = new NLog.Config.LoggingConfiguration();
        NLog.Layouts.Layout layout;
        if (minConsoleLogLevel <= NLog.LogLevel.Debug)
        {
            layout = NLog.Layouts.Layout.FromString("${longdate}|${level:uppercase=true}|${callsite}:${callsite-linenumber}|${message}${onexception:${newline}${exception:format=tostring}${exception:format=StackTrace}}");
        }
        else
        {
            layout = NLog.Layouts.Layout.FromString("${longdate}|${message}${onexception:${newline}${exception:format=tostring}${exception:format=StackTrace}}");
        }
        logConfig.AddTarget("console", new NLog.Targets.ColoredConsoleTarget() { Layout = layout });
        logConfig.AddRule(minConsoleLogLevel, NLog.LogLevel.Fatal, "console");
        NLog.LogManager.Configuration = logConfig;
    }
}