using CommandLine;
using Luban.CodeTarget.CSharp;
using Luban.DataExporter.Builtin;
using Luban.DataLoader.Builtin;
using Luban.DataValidator.Builtin.Collection;
using Luban.Pipeline;
using Luban.Schema.Builtin;
using Luban.Utils;
using NLog;
using System.Reflection;
using System.Text;

namespace Luban;

internal static class Program
{

    private class CommandOptions
    {
        
        [Option('s', "schemaCollector", Required = false, HelpText = "schema collector name")]
        public string SchemaCollector { get; set; } = "default";
        
        [Option("schemaPath", Required = true, HelpText = "schema path")]
        public string SchemaPath { get; set; }
        
        [Option('t', "target", Required = true, HelpText = "target name")]
        public string Target { get; set; }
        
        [Option('c', "codeTarget", Required = false, HelpText = "code target name")]
        public IEnumerable<string> CodeTargets { get; set; }
        
        [Option('d', "dataTarget", Required = false, HelpText = "data target name")]
        public IEnumerable<string> DataTargets { get; set; }

        [Option('p', "pipeline", Required = false, HelpText = "pipeline name")]
        public string Pipeline { get; set; } = "default";
        
        [Option('i', "includeTag", Required = false, HelpText = "include tag")]
        public IEnumerable<string> IncludeTags { get; set; }

        [Option('e', "excludeTag", Required = false, HelpText = "exclude tag")]
        public IEnumerable<string> ExcludeTags { get; set; }
        
        [Option("outputTable", Required = false, HelpText = "output table")]
        public IEnumerable<string> OutputTables { get; set; }
        
        [Option("timeZone", Required = false, HelpText = "time zone")]
        public string TimeZone { get; set; }
        
        [Option('x', "xargs", Required = false, HelpText = "args like -x a=1 -x b=2")]
        public IEnumerable<string> Xargs { get; set; }
        
        [Option('v', "verbose", Required = false, HelpText = "verbose")]
        public bool Verbose { get; set; }
    }
    
    private static ILogger s_logger;

    private static void Main(string[] args)
    {
        CommandOptions opts = ParseArgs(args);
        SetupApp(opts);
        
        var launcher = new SimpleLauncher();
        var builtinAssemblies = new List<Assembly>
        {
            typeof(CsharpBinCodeTarget).Assembly,
            typeof(DefaultSchemaCollector).Assembly,
            typeof(FieldNames).Assembly,
            typeof(DefaultDataExporter).Assembly,
            typeof(SizeValidator).Assembly,
        };
        launcher.Start(builtinAssemblies, ParseXargs(opts.Xargs));
        
        var pipeline = PipelineManager.Ins.CreatePipeline(opts.Pipeline);
        pipeline.Run(CreatePipelineArgs(opts));
        s_logger.Info("bye~");
    }

    private static CommandOptions ParseArgs(string[] args)
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
        return ((Parsed<CommandOptions>)result).Value;
    }
    
    private static Dictionary<string, string> ParseXargs(IEnumerable<string> xargs)
    {
        var result = new Dictionary<string, string>();
        foreach (var xarg in xargs)
        {
            string[] pair = xarg.Split('=', 2);
            result.Add(pair[0], pair[1]);
        }
        return result;
    }

    private static PipelineArguments CreatePipelineArgs(CommandOptions opts)
    {
        return new PipelineArguments()
        {
            Target = opts.Target,
            SchemaCollector = opts.SchemaCollector,
            SchemaPath = opts.SchemaPath,
            OutputTables = opts.OutputTables?.ToList() ?? new List<string>(),
            CodeTargets = opts.CodeTargets?.ToList() ?? new List<string>(),
            DataTargets = opts.DataTargets?.ToList() ?? new List<string>(),
            IncludeTags = opts.IncludeTags?.ToList() ?? new List<string>(),
            ExcludeTags = opts.ExcludeTags?.ToList() ?? new List<string>(),
            TimeZone = opts.TimeZone,
        };
    }

    private static void SetupApp(CommandOptions opts)
    {
        ConsoleUtil.EnableQuickEditMode(false);
        Console.OutputEncoding = Encoding.UTF8;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        int processorCount = Environment.ProcessorCount;
        ThreadPool.SetMinThreads(Math.Max(4, processorCount), 0);
        ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 2), 2);

        InitSimpleNLogConfigure(opts.Verbose ? LogLevel.Trace : LogLevel.Info);
        s_logger = LogManager.GetCurrentClassLogger();
        PrintCopyRight();
    }

    private static void PrintCopyRight()
    {
        s_logger.Info(" =============================================================================================");
        s_logger.Info(" ");
        s_logger.Info("    Luban is developed by Code Philosophy Technology Co., LTD. (https://code-philosophy.com/)");
        s_logger.Info(" ");
        s_logger.Info(" =============================================================================================");
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