using Bright.Common;
using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.IO;
using System.Reflection;
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
        }

        private static CommandLineOptions ParseOptions(String[] args)
        {
            var helpWriter = new StringWriter();
            var parser = new Parser(ps =>
            {
                ps.HelpWriter = helpWriter;
            });

            var result = parser.ParseArguments<CommandLineOptions>(args);
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.Error.WriteLine(helpWriter.ToString());
                Environment.Exit(1);
            }
            return ((Parsed<CommandLineOptions>)result).Value;
        }

        static void Main(string[] args)
        {
            ConsoleWindow.EnableQuickEditMode(false);
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var options = ParseOptions(args);

            if (!string.IsNullOrEmpty(options.TemplateSearchPath))
            {
                StringTemplateUtil.AddTemplateSearchPath(options.TemplateSearchPath);
            }
            StringTemplateUtil.AddTemplateSearchPath(FileUtil.GetPathRelateApplicationDirectory("Templates"));

            Luban.Common.Utils.LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.FromString(options.LogLevel));

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            GenServer.Ins.Start(options.Port, ProtocolStub.Factories);

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
