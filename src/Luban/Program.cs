// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using CommandLine;
using Luban.DataLoader;
using Luban.Diagnostics;
using Luban.Pipeline;
using Luban.Schema;
using Luban.Tmpl;
using Luban.Utils;
using NLog;
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

        [Option("variant", Required = false, HelpText = "field variants")]
        public IEnumerable<string> Variants { get; set; }

        [Option('o', "outputTable", Required = false, HelpText = "output table")]
        public IEnumerable<string> OutputTables { get; set; }

        [Option("timeZone", Required = false, HelpText = "time zone")]
        public string TimeZone { get; set; }

        [Option("customTemplateDir", Required = false, HelpText = "custom template dirs")]
        public IEnumerable<string> CustomTemplateDirs { get; set; }

        [Option("strict", Required = false, HelpText = "treat validation failure as error")]
        public bool Strict { get; set; }

        [Option("locale", Required = false, HelpText = "locale for error/warning messages (en, zh). default: system UI language")]
        public string Locale { get; set; }

        [Option("errorFormat", Required = false, Default = "text", HelpText = "error output format: text|json (json is for AI/CI tooling)")]
        public string ErrorFormat { get; set; } = "text";

        [Option('x', "xargs", Required = false, HelpText = "args like -x a=1 -x b=2")]
        public IEnumerable<string> Xargs { get; set; }

        [Option('l', "logConfig", Required = false, Default = "nlog.xml", HelpText = "nlog config file")]
        public string LogConfig { get; set; }

        [Option('w', "watchDir", Required = false, HelpText = "watch dir and regererate when dir changes")]
        public IEnumerable<string> WatchDirs { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "verbose")]
        public bool Verbose { get; set; }
    }

    private static ILogger s_logger;

    private static void Main(string[] args)
    {
        CommandOptions opts = ParseArgs(args);
        SetupApp(opts);

        if (opts.WatchDirs != null && opts.WatchDirs.Any())
        {
            RunLoop(opts, opts.WatchDirs);
        }
        else
        {
            RunOnce(opts);
        }
    }

    private static void RunOnce(CommandOptions opts)
    {
        RunGeneration(opts, true);
    }

    private static volatile bool s_anyChange = false;

    private static void RunLoop(CommandOptions opts, IEnumerable<string> watchDirs)
    {
        var watcher = new DirectoryWatcher(opts.WatchDirs.ToArray(), () => s_anyChange = true);
        s_anyChange = true;
        while (true)
        {
            if (s_anyChange)
            {
                s_anyChange = false;
                RunGeneration(opts, false);
            }
            Thread.Sleep(1000);
        }
    }

    private static bool UseJsonErrors(CommandOptions opts)
        => string.Equals(opts.ErrorFormat, "json", StringComparison.OrdinalIgnoreCase);

    private static void EmitErrorReport(DiagnosticReport report, CommandOptions opts)
    {
        if (UseJsonErrors(opts))
        {
            Console.Error.WriteLine(report.ToJson());
            return;
        }
        foreach (var err in report.Errors)
        {
            s_logger.Error("[{}] {}{}", err.Category, err.Code != null ? err.Code + ": " : "", err.Message);
            if (!string.IsNullOrEmpty(err.File))
            {
                s_logger.Error("  file: {}", err.File);
            }
            if (!string.IsNullOrEmpty(err.Location))
            {
                s_logger.Error("  location: {}", err.Location);
            }
            if (!string.IsNullOrEmpty(err.FieldPath))
            {
                s_logger.Error("  field: {}", err.FieldPath);
            }
        }
    }

    private static void RunGeneration(CommandOptions opts, bool exitOnError)
    {
        try
        {
            IConfigLoader rootLoader = new GlobalConfigLoader();
            var config = rootLoader.Load(opts.ConfigFile);

            using var scope = PipelineScope.Create(ParseXargs(config.Xargs, opts.Xargs));
            using (scope.Enter())
            {
                scope.Config = config;
                AddCustomTemplateDirs(opts.CustomTemplateDirs);

                var pipeline = scope.Pipelines.CreatePipeline(opts.Pipeline);
                scope.Pipeline = pipeline;
                pipeline.Run(CreatePipelineArgs(opts, config));
                if (exitOnError && opts.Strict && scope.GenerationContext.AnyValidatorFail)
                {
                    var report = DiagnosticReport.ValidationFailed();
                    if (UseJsonErrors(opts))
                    {
                        EmitErrorReport(report, opts);
                    }
                    else
                    {
                        s_logger.Error(MessageCatalog.Format("error.cli.validation_fail"));
                    }
                    Environment.Exit(1);
                }
                if (UseJsonErrors(opts) && exitOnError)
                {
                    Console.Error.WriteLine(DiagnosticReport.Success().ToJson());
                }
                s_logger.Info("bye~");
            }
        }
        catch (Exception e)
        {
            if (UseJsonErrors(opts))
            {
                EmitErrorReport(DiagnosticReport.FromException(e), opts);
            }
            else
            {
                PrettyPrintException(e);
                s_logger.Error(MessageCatalog.Format("error.cli.run_failed"));
            }
            if (exitOnError)
            {
                Environment.Exit(1);
            }
        }
    }

    private static void PrettyPrintException(Exception e)
    {
        if (TryExtractDataCreateException(e, out var dce))
        {
            s_logger.Error("=======================================================================");
            s_logger.Error(MessageCatalog.Format("error.data.parse_failed"));
            s_logger.Error(MessageCatalog.Format("error.data.parse_file", dce.OriginDataLocation));
            s_logger.Error(MessageCatalog.Format("error.data.parse_location", dce.DataLocationInFile));
            s_logger.Error(MessageCatalog.Format("error.data.parse_err", dce.OriginErrorMsg));
            s_logger.Error(MessageCatalog.Format("error.data.parse_field", dce.VariableFullPathStr));
            s_logger.Error("=======================================================================");
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


    private static Dictionary<string, string> ParseXargs0(IEnumerable<string> xargs)
    {
        var result = new Dictionary<string, string>();
        if (xargs == null)
        {
            return result;
        }
        foreach (var arg in xargs)
        {
            string[] pair = arg.Split('=', 2);
            if (pair.Length != 2)
            {
                throw new LubanException("error.cli.invalid_xargs", arg);
            }

            if (!result.TryAdd(pair[0], pair[1]))
            {
                throw new LubanException("error.cli.duplicate_xargs", arg);
            }
        }
        return result;
    }

    private static Dictionary<string, string> ParseXargs(IEnumerable<string> defaultXargs, IEnumerable<string> cmdXargs)
    {
        var defaultXargsMap = ParseXargs0(defaultXargs);
        var cmdXargsMap = ParseXargs0(cmdXargs);
        foreach (var kv in cmdXargsMap)
        {
            defaultXargsMap[kv.Key] = kv.Value;
        }
        return defaultXargsMap;
    }

    private static Dictionary<string, string> ParseVariants(IEnumerable<string> variants)
    {
        var result = new Dictionary<string, string>();
        if (variants == null)
        {
            return result;
        }
        foreach (var variant in variants)
        {
            string[] pair = variant.Split('=', 2);
            if (pair.Length != 2)
            {
                throw new LubanException("error.cli.invalid_variant", variant);
            }

            if (!result.TryAdd(pair[0], pair[1]))
            {
                throw new LubanException("error.cli.duplicate_variant", variant);
            }
        }
        return result;
    }

    private static PipelineArguments CreatePipelineArgs(CommandOptions opts, LubanConfig config)
    {
        return new PipelineArguments()
        {
            Target = opts.Target,
            ForceLoadTableDatas = opts.ForceLoadTableDatas,
            SchemaCollector = opts.SchemaCollector,
            Config = config,
            OutputTables = opts.OutputTables?.ToList() ?? new List<string>(),
            CodeTargets = opts.CodeTargets?.ToList() ?? new List<string>(),
            DataTargets = opts.DataTargets?.ToList() ?? new List<string>(),
            IncludeTags = opts.IncludeTags?.ToList() ?? new List<string>(),
            ExcludeTags = opts.ExcludeTags?.ToList() ?? new List<string>(),
            Variants = ParseVariants(opts.Variants),
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
        MessageCatalog.Init(opts.Locale);
        PrintCopyRight();
    }

    private static void PrintCopyRight()
    {
        s_logger.Info(" ==========================================================================================");
        s_logger.Info("");
        s_logger.Info("  Luban is developed by Code Philosophy Technology Co., LTD. https://code-philosophy.com");
        s_logger.Info("  Github: https://github.com/focus-creative-games/luban");
        s_logger.Info("  Document: https://www.datable.cn");
        s_logger.Info("");
        s_logger.Info(" ==========================================================================================");
    }
}
