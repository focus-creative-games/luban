using CommandLine;
using Luban.CSharp;
using Luban.DataExporter.Builtin;
using Luban.DataLoader.Builtin;
using Luban.DataLoader.Builtin.DataVisitors;
using Luban.DataValidator.Builtin.Collection;
using Luban.L10N;
using Luban.Pipeline;
using Luban.Protobuf.TypeVisitors;
using Luban.Schema.Builtin;
using Luban.Tmpl;
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

        [Option("conf", Required = true, HelpText = "luban conf file")]
        public string ConfigFile { get; set; }

        [Option('t', "target", Required = true, HelpText = "target name")]
        public string Target { get; set; }

        [Option('c', "codeTarget", Required = false, HelpText = "code target name")]
        public IEnumerable<string> CodeTargets { get; set; }

        [Option('d', "dataTarget", Required = false, HelpText = "data target name")]
        public IEnumerable<string> DataTargets { get; set; }

        [Option('p', "pipeline", Required = false, HelpText = "pipeline name")]
        public string Pipeline { get; set; } = "default";

        [Option('f', "forceLoadTableDatas", Required = false, HelpText = "force load table datas when not any dataTarget")]
        public bool ForceLoadTableDatas { get; set; }

        [Option('i', "includeTag", Required = false, HelpText = "include tag")]
        public IEnumerable<string> IncludeTags { get; set; }

        [Option('e', "excludeTag", Required = false, HelpText = "exclude tag")]
        public IEnumerable<string> ExcludeTags { get; set; }

        [Option('o', "outputTable", Required = false, HelpText = "output table")]
        public IEnumerable<string> OutputTables { get; set; }

        [Option("timeZone", Required = false, HelpText = "time zone")]
        public string TimeZone { get; set; }

        [Option("customTemplateDir", Required = false, HelpText = "custom template dirs")]
        public IEnumerable<string> CustomTemplateDirs { get; set; }

        [Option("validationFailAsError", Required = false, HelpText = "validation fail as error")]
        public bool ValidationFailAsError { get; set; }

        [Option('x', "xargs", Required = false, HelpText = "args like -x a=1 -x b=2")]
        public IEnumerable<string> Xargs { get; set; }

        [Option('l', "logConfig", Required = false, Default = "nlog.xml", HelpText = "nlog config file")]
        public string LogConfig { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "verbose")]
        public bool Verbose { get; set; }
    }

    private static ILogger s_logger;

    private static void Main(string[] args)
    {
        CommandOptions opts = ParseArgs(args);
        SetupApp(opts);

        try
        {
            var launcher = new SimpleLauncher();
            launcher.Start(ParseXargs(opts.Xargs));
            AddCustomTemplateDirs(opts.CustomTemplateDirs);

            var pipeline = PipelineManager.Ins.CreatePipeline(opts.Pipeline);
            pipeline.Run(CreatePipelineArgs(opts));
            if (opts.ValidationFailAsError && GenerationContext.Current.AnyValidatorFail)
            {
                s_logger.Error("encounter some validation failure. exit code: 1");
                Environment.Exit(1);
            }
            s_logger.Info("bye~");
        }
        catch (Exception e)
        {
            PrettyPrintException(e);
            s_logger.Error("run failed!!!");
            Environment.Exit(1);
        }
    }

    private static void PrettyPrintException(Exception e)
    {
        if (TryExtractDataCreateException(e, out var dce))
        {
            s_logger.Error($"=======================================================================");
            s_logger.Error("解析失败!");
            s_logger.Error($"文件:        {dce.OriginDataLocation}");
            s_logger.Error($"错误位置:    {dce.DataLocationInFile}");
            s_logger.Error($"Err:         {dce.OriginErrorMsg}");
            s_logger.Error($"字段:        {dce.VariableFullPathStr}");
            s_logger.Error($"=======================================================================");
            return;
        }
        do
        {
            s_logger.Error("===> {}", e.Message);
            e = e.InnerException;
        } while (e != null);
    }

    private static bool TryExtractDataCreateException(Exception e, out DataCreateException extract)
    {
        if (e is DataCreateException dce)
        {
            extract = dce;
            return true;
        }

        if (e is AggregateException ae)
        {
            foreach (var innerException in ae.InnerExceptions)
            {
                if (TryExtractDataCreateException(innerException, out extract))
                {
                    return true;
                }
            }
        }

        if (e.InnerException != null)
        {
            if (TryExtractDataCreateException(e.InnerException, out extract))
            {
                return true;
            }
        }
        extract = null;
        return false;
    }

    private static void AddCustomTemplateDirs(IEnumerable<string> dirs)
    {
        foreach (var dir in dirs)
        {
            TemplateManager.Ins.AddTemplateSearchPath(dir, true, true);
        }
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
        foreach (var arg in xargs)
        {
            string[] pair = arg.Split('=', 2);
            if (pair.Length != 2)
            {
                Console.Error.WriteLine($"invalid xargs:{arg}");
                Environment.Exit(1);
            }

            if (!result.TryAdd(pair[0], pair[1]))
            {
                Console.Error.WriteLine($"duplicate xargs:{arg}");
                Environment.Exit(1);
            }
        }
        return result;
    }

    private static PipelineArguments CreatePipelineArgs(CommandOptions opts)
    {
        return new PipelineArguments()
        {
            Target = opts.Target,
            ForceLoadTableDatas = opts.ForceLoadTableDatas,
            SchemaCollector = opts.SchemaCollector,
            ConfFile = opts.ConfigFile,
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

        NLog.LogManager.Setup().LoadConfigurationFromFile(opts.LogConfig);
        s_logger = LogManager.GetCurrentClassLogger();
        PrintCopyRight();
    }

    private static void PrintCopyRight()
    {
        s_logger.Info(" ==========================================================================================");
        s_logger.Info("");
        s_logger.Info("  Luban is developed by Code Philosophy Technology Co., LTD. https://code-philosophy.com");
        s_logger.Info("");
        s_logger.Info(" ==========================================================================================");
    }
}
