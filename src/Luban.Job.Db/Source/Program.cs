//using Bright.Common;
//using CommandLine;
//using CommandLine.Text;
//using Gen.Client.Common.Net;
//using Gen.Client.Common.Utils;
//using Gen.Db.Common.RawDefs;
//using Luban.Job.Common.Net;
//using Luban.Job.Common.Utils;
//using System;

//namespace Luban.Job.Db.Client
//{
//    public class CommandLineOptions
//    {
//        [Option('h', "host", Required = true, HelpText = "gen server host")]
//        public string Host { get; set; }

//        [Option('p', "port", Required = false, HelpText = "gen server port")]
//        public int Port { get; set; } = 8899;

//        [Option('d', "define", Required = true, HelpText = "define file")]
//        public string DefineFile { get; set; }

//        [Option('c', "outputcodedir", Required = true, HelpText = "output code directory")]
//        public string OutputCodeDir { get; set; }

//        [Option('l', "language", Required = true, HelpText = "code language. only support cs currently")]
//        public string Languange { get; set; } = "cs";

//        [Option('v', "verbose", Required = false, HelpText = "verbose output")]
//        public bool Verbose { get; set; }

//        [Option("cachemetainfofile", Required = false, HelpText = "cache meta info file")]
//        public string CacheMetaInfoFile { get; set; } = ".cache.meta";
//    }

//    class Program
//    {
//        private static NLog.Logger s_logger;

//        private static CommandLineOptions _options;

//        static void Main(string[] args)
//        {
//            var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);

//            parseResult.WithNotParsed(errs =>
//            {
//                Environment.Exit(1);
//            });

//            parseResult.WithParsed(opts =>
//            {
//                _options = opts;
//            });

//            LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.Info);

//            s_logger = NLog.LogManager.GetCurrentClassLogger();

//            int exitCode = 0;
//            try
//            {
//                var timer = new ProfileTimer();
//                timer.StartPhase("total");

//                long beginTime = Bright.Time.TimeUtil.NowMillis;

//                timer.StartPhase("load cache meta file");
//                CacheMetaManager.Ins.Load(_options.CacheMetaInfoFile);
//                timer.EndPhaseAndLog();

//                RpcManager.Ins.Start();
//                var conn = GenClient.Ins.Start(_options.Host, _options.Port, ProtocolStub.Factories, _options.Verbose);
//                conn.Wait();

//                timer.StartPhase("load defines");
//                var loader = new DbDefLoader();
//                loader.Load(_options.DefineFile);
//                timer.EndPhaseAndLog();

//                timer.StartPhase("call GenDb");
//                string outputDir = _options.OutputCodeDir;

//                var rpc = new GenDb();
//                var res = rpc.Call(GenClient.Ins.Session, new GenDbArg()
//                {
//                    Define = loader.BuildDefines(),
//                    OutputCodeRelatePath = outputDir,

//                }).Result;
//                timer.EndPhaseAndLog();

//                if (res.OK)
//                {
//                    timer.StartPhase("fetch generated files");
//                    DownloadFileUtil.DownloadGeneratedFiles(outputDir, res.NewCodeFiles).Wait();
//                    timer.EndPhaseAndLog();

//                    CacheMetaManager.Ins.Save();
//                    timer.EndPhaseAndLog();
//                    s_logger.Info("==== run succ ====");
//                }
//                else
//                {
//                    timer.EndPhaseAndLog();
//                    s_logger.Error("==== run fail ====");
//                    exitCode = 1;
//                }
//            }
//            catch (Exception e)
//            {
//                s_logger.Error(e, "==== run fail ====");
//                exitCode = 1;
//            }
//            Environment.Exit(exitCode);
//        }
//    }
//}
