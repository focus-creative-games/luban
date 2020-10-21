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

        private static CommandLineOptions options;

        static void Main(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);

            parseResult.WithNotParsed(errs =>
            {
                Environment.Exit(-1);
            });

            parseResult.WithParsed(opts =>
            {
                options = opts;
            });

            Luban.Common.Utils.LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.Info);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            GenServer.Ins.Start(options.Port, ProtocolStub.Factories);
#if EMBED_CFG
            GenServer.Ins.RegisterJob("cfg", new Luban.Job.Cfg.JobController());
#endif
            int processorCount = System.Environment.ProcessorCount;
            ThreadPool.SetMinThreads(Math.Max(4, processorCount), 5);
            ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 4), 10);

            Thread.CurrentThread.Join();
        }

    }
}
