using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
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

            [Option("i10n:default_timezone", Required = false, HelpText = "default timezone")]
            public string L10nDefaultTimeZone { get; set; } = "Asia/Shanghai";
        }

        static void Main(string[] args)
        {
            ConsoleWindow.EnableQuickEditMode(false);
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var options = CommandLineUtil.ParseOptions<CommandLineOptions>(args);

            if (!string.IsNullOrEmpty(options.TemplateSearchPath))
            {
                StringTemplateUtil.AddTemplateSearchPath(options.TemplateSearchPath);
            }
            StringTemplateUtil.AddTemplateSearchPath(FileUtil.GetPathRelateApplicationDirectory("Templates"));

            Luban.Common.Utils.LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.FromString(options.LogLevel));

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            TimeZoneUtil.DefaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById(options.L10nDefaultTimeZone);

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
