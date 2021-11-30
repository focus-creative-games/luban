using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Cfg.Cache;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using System;
using System.IO;
using System.Threading;

namespace Luban.Server
{
    class Program
    {
        class CommandLineOptions
        {
            [Option('p', "port", Required = false, HelpText = "listen port")]
            public int Port { get; set; } = 8899;

            [Option('l', "loglevel", Required = false, HelpText = "log level. default INFO. avaliable value: TRACE,DEBUG,INFO,WARN,ERROR,FATAL,OFF")]
            public string LogLevel { get; set; } = "INFO";

            [Option('t', "template_search_path", Required = false, HelpText = "additional template search path")]
            public string TemplateSearchPath { get; set; }

            [Option("disable_cache", Required = false, HelpText = "disable generation cache")]
            public bool DisableCache { get; set; }

            [Option("i10n:default_timezone", Required = false, HelpText = "default timezone id. 'Asia/Shanghai', 'China Standard Time' eg. you can also use two special values: local,utc")]
            public string L10nDefaultTimeZone { get; set; }
        }

        static void Main(string[] args)
        {
            ConsoleWindow.EnableQuickEditMode(false);
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var options = CommandLineUtil.ParseOptions<CommandLineOptions>(args);

            FileRecordCacheManager.Ins.Init(!options.DisableCache);

            StringTemplateManager.Ins.Init(!options.DisableCache);
            if (!string.IsNullOrEmpty(options.TemplateSearchPath))
            {
                StringTemplateManager.Ins.AddTemplateSearchPath(options.TemplateSearchPath);
            }
            StringTemplateManager.Ins.AddTemplateSearchPath(FileUtil.GetPathRelateApplicationDirectory("Templates"));

            Luban.Common.Utils.LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.FromString(options.LogLevel));

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            TimeZoneUtil.InitDefaultTimeZone(options.L10nDefaultTimeZone);

            GenServer.Ins.Start(false, options.Port, ProtocolStub.Factories);

            GenServer.Ins.RegisterJob("cfg", new Luban.Job.Cfg.JobController());
            GenServer.Ins.RegisterJob("proto", new Luban.Job.Proto.JobController());
            GenServer.Ins.RegisterJob("db", new Luban.Job.Db.JobController());

            int processorCount = System.Environment.ProcessorCount;
            ThreadPool.SetMinThreads(Math.Max(4, processorCount), 5);
            ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 4), 10);

            Console.WriteLine("== running ==");
        }

    }
}
