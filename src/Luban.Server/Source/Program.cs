using Bright.Common;
using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
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
            var options = ParseOptions(args);

            Luban.Common.Utils.LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.Info);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            GenServer.Ins.Start(options.Port, ProtocolStub.Factories);
#if EMBED_CFG
            GenServer.Ins.RegisterJob("cfg", new Luban.Job.Cfg.JobController());
#endif
            GenServer.Ins.RegisterJob("proto", new Luban.Job.Proto.JobController());
            GenServer.Ins.RegisterJob("db", new Luban.Job.Db.JobController());
            int processorCount = System.Environment.ProcessorCount;
            ThreadPool.SetMinThreads(Math.Max(4, processorCount), 5);
            ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 4), 10);

            Thread.CurrentThread.Join();
        }

    }
}
