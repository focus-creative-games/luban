using CommandLine;
using Luban.Client.Common.Net;
using Luban.Client.Common.Utils;
using Luban.Client.Utils;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Cfg.Cache;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using Luban.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luban.ClientServer
{
    class Program
    {
        public class AllCommandLineOptions
        {
            public string Host { get; set; }

            public int Port { get; set; } = 8899;

            public string JobType { get; set; }

            public List<string> JobArguments { get; set; } = new List<string>();

            public bool Verbose { get; set; }

            public string LogLevel { get; set; } = "INFO";

            public string CacheMetaInfoFile { get; set; } = ".cache.meta";

            public string[] WatchDir { get; set; }

            public bool GenerateOnly { get; set; }

            [Option('t', "template_search_path", Required = false, HelpText = "string template search path.")]
            public string TemplateSearchPath { get; set; }
        }

        private static void PrintUsage(string err)
        {
            Console.WriteLine("ERRORS:");
            Console.WriteLine("\t" + err);
            Console.WriteLine(@"
Luban.ClientServer <Options>...  [-- [job options]]
e.g.
    Luban.ClientServer -j cfg --  --name abc

Options:
  -h, --host  <host>            host. default use embed Luban.Server
  -p  --port  <port>            port. default 8899
  -j  --job   <job>             required. job type.  avaliable value: cfg
  -v  --verbose                 verbose print
  -l  --loglevel <level>        log level. default INFO. avaliable value: TRACE,DEBUG,INFO,WARN,ERROR,FATAL,OFF
  -c  --cachemetafile <file>    cache meta file name. default is '.cache.meta'
  -w  --watch  <dir>            watch data change and regenerate.
  --generateonly                generate only. not download generate results.
  -t  --template_search_path <dir> additional template search path
  -h  --help                    show usage
");
        }

        private static (object, AllCommandLineOptions) ParseArgs(string[] args)
        {
            var ops = new AllCommandLineOptions();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                try
                {
                    switch (arg)
                    {
                        case "-h":
                        case "--host":
                        {
                            // 打个补丁。好多人忘了设置 LUBAN_SERVER_IP 环境变量，导致启动时出问题
                            if (args[i + 1].StartsWith("-"))
                            {
                                //Console.WriteLine("[WARN] --host (or -h) <LUBAN_SERVER_IP> 参数丢失, use 127.0.0.1 as default. do you forget to set LUBAN_SERVER_IP env variable?");
                                //ops.Host = "127.0.0.1";
                                Console.WriteLine("[WARN] 对于Luban.ClientServer，参数 {0} 没有意义，忽略。", arg);
                            }
                            else
                            {
                                string argv = args[++i];
                                Console.WriteLine("[WARN] 对于Luban.ClientServer，参数 {0} {1} 没有意义，忽略。", arg, argv);
                            }
                            break;
                        }
                        case "-p":
                        case "--port":
                        {
                            ops.Port = int.Parse(args[++i]);
                            break;
                        }
                        case "-j":
                        case "--job":
                        {
                            ops.JobType = args[++i];
                            break;
                        }
                        case "-v":
                        case "--verbose":
                        {
                            ops.Verbose = true;
                            break;
                        }
                        case "-l":
                        case "--loglevel":
                        {
                            ops.LogLevel = args[++i];
                            break;
                        }
                        case "-c":
                        case "--cachemetafile":
                        {
                            ops.CacheMetaInfoFile = args[++i];
                            break;
                        }
                        case "-w":
                        case "--watch":
                        {
                            ops.WatchDir = args[++i].Split(';', ',');
                            break;
                        }
                        case "--generateonly":
                        {
                            ops.GenerateOnly = true;
                            break;
                        }
                        case "-t":
                        case "--template_search_path":
                        {
                            var dirName = args[++i];
                            if (Directory.Exists(dirName))
                            {
                                ops.TemplateSearchPath = dirName;
                            }
                            else
                            {
                                Console.WriteLine("[WARN] 对于Luban.ClientServer，参数 {0} {1} 路径不存在，忽略。", arg, dirName);
                            }
                            break;
                        }
                        case "--":
                        {
                            ++i;
                            ops.JobArguments = args.ToList().GetRange(i, args.Length - i);
                            return (null, ops);
                        }
                        default:
                        {
                            return ($"unknown argument:{arg}", null);
                        }
                    }
                }
                catch (Exception)
                {
                    return ($"argument:{arg} err", null);
                }
            }
            if (ops.JobType == null)
            {
                return ("--job missing", null);
            }

            return (null, ops);
        }

        private static void StartServer(AllCommandLineOptions options)
        {
            FileRecordCacheManager.Ins.Init(true);
            StringTemplateManager.Ins.Init(true);
            if (!string.IsNullOrEmpty(options.TemplateSearchPath))
            {
                StringTemplateManager.Ins.AddTemplateSearchPath(options.TemplateSearchPath);
            }
            StringTemplateManager.Ins.AddTemplateSearchPath(FileUtil.GetPathRelateApplicationDirectory("Templates"));

            GenServer.Ins.Start(true, options.Port, ProtocolStub.Factories);

            GenServer.Ins.RegisterJob("cfg", new Luban.Job.Cfg.JobController());
            GenServer.Ins.RegisterJob("proto", new Luban.Job.Proto.JobController());
            GenServer.Ins.RegisterJob("db", new Luban.Job.Db.JobController());
        }

        private static void StartClient(AllCommandLineOptions options, ProfileTimer profile)
        {
            if (options.WatchDir == null || options.WatchDir.Length == 0)
            {
                int exitCode = GenOnce(options, profile);
                Environment.Exit(exitCode);
            }
            else
            {
                GenOnce(options, profile);
                new MultiFileWatcher(options.WatchDir, () => GenOnce(options, profile));
            }
        }

        private static NLog.Logger s_logger;

        static void Main(string[] args)
        {
            ConsoleWindow.EnableQuickEditMode(false);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var parseResult = ParseArgs(args);
            if (parseResult.Item1 != null)
            {
                PrintUsage((string)parseResult.Item1);
                Environment.Exit(1);
                return;
            }
            var options = parseResult.Item2;

            var profile = new ProfileTimer();
            profile.StartPhase("all");

            LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.FromString(options.LogLevel));
            s_logger = NLog.LogManager.GetCurrentClassLogger();

            TimeZoneUtil.InitDefaultTimeZone("");

            int processorCount = System.Environment.ProcessorCount;
            ThreadPool.SetMinThreads(Math.Max(4, processorCount), 5);
            ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 4), 10);

            if (string.IsNullOrEmpty(options.TemplateSearchPath))
            {
                options.TemplateSearchPath = FileUtil.GetPathRelateApplicationDirectory("Templates");
            }
            if (string.IsNullOrWhiteSpace(options.Host))
            {
                options.Host = "127.0.0.1";
                StartServer(options);
            }

            StartClient(options, profile);
        }

        private static int GenOnce(AllCommandLineOptions options, ProfileTimer profile)
        {
            int exitCode;
            try
            {
                profile.StartPhase("generation");
                var conn = GenClient.Start(options.Host, options.Port, ProtocolStub.Factories);

                profile.StartPhase("load cache meta file");
                CacheMetaManager.Ins.Load(options.CacheMetaInfoFile);
                profile.EndPhaseAndLog();

                conn.Wait();

                if (GenClient.Ins.Session.Channel.IsOpen)
                {
                    profile.StartPhase("gen job");
                    exitCode = SubmitGenJob(options);
                    profile.EndPhaseAndLog();
                }
                else
                {
                    s_logger.Error("connect fail");
                    exitCode = 2;
                }

                profile.EndPhaseAndLog();
            }
            catch (Exception e)
            {
                exitCode = 1;
                s_logger.Error(e);
            }
            finally
            {
                GenClient.Stop();
            }

            CacheMetaManager.Ins.Save();
            CacheMetaManager.Ins.Reset();
            if (exitCode == 0)
            {
                s_logger.Info("== succ ==");
            }
            else
            {
                s_logger.Error("== fail ==");
            }
            return exitCode;
        }

        const int GEN_JOB_TIMEOUT = 120;

        private static int SubmitGenJob(AllCommandLineOptions options)
        {
            var res = GenClient.Ins.Session.CallRpcAsync<GenJob, GenJobArg, GenJobRes>(new GenJobArg()
            {
                JobType = options.JobType,
                JobArguments = options.JobArguments,
                Verbose = options.Verbose,
            }, GEN_JOB_TIMEOUT).Result;

            if (res.ErrCode != 0)
            {
                if (res.ErrCode == Luban.Common.EErrorCode.JOB_ARGUMENT_ERROR)
                {
                    s_logger.Error("job argument error");
                    Console.WriteLine(res.ErrMsg);
                }
                else
                {
                    s_logger.Error("GenJob fail. err:{err} msg:{msg}", res.ErrCode, res.ErrMsg);
                    if (!string.IsNullOrEmpty(res.StackTrace))
                    {
                        s_logger.Debug("StackTrace: {}", res.StackTrace);
                    }
                }

                return 1;
            }

            if (!options.GenerateOnly)
            {
                var tasks = new List<Task>();

                foreach (var fg in res.FileGroups)
                {
                    tasks.Add(DownloadFileUtil.DownloadGeneratedFiles(fg.Dir, fg.Files));
                }

                foreach (var f in res.ScatteredFiles)
                {
                    tasks.Add(DownloadFileUtil.DownloadGeneratedFile(f));
                }

                Task.WaitAll(tasks.ToArray());
            }
            return 0;
        }
    }
}