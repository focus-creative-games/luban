using Luban.Client.Common.Net;
using Luban.Client.Common.Utils;
using Luban.Common.Protos;
using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luban.Client
{
    class Program
    {
        public class CommandLineOptions
        {
            public string Host { get; set; }

            public int Port { get; set; } = 8899;

            public string JobType { get; set; }

            public List<string> JobArguments { get; set; } = new List<string>();

            public bool Verbose { get; set; }

            public string CacheMetaInfoFile { get; set; } = ".cache.meta";
        }

        private static NLog.Logger s_logger;

        private static void PrintUsage(string err)
        {
            Console.WriteLine("ERRORS:");
            Console.WriteLine("\t" + err);
            Console.WriteLine(@"
Luban.Client <Options>...  [-- [job options]]
e.g.
    Luban.Client -h 127.0.0.1 -p 2234 -j cfg --  --name abc

Options:
  -h, --host            Required. host ip
  -p  --port            port. default 8899
  -j  --job             Required. job type.  avaliable value: cfg
  -v  --verbose         verbose print
  -c  --cachemetafile   cache meta file name
  -h  --help            show usage
");
        }

        private static (object, CommandLineOptions) ParseArgs(string[] args)
        {
            var ops = new CommandLineOptions();

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

                            ops.Host = args[++i];
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
                        case "-c":
                        case "--cachemetafile":
                        {
                            ops.CacheMetaInfoFile = args[++i];
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
            if (ops.Host == null)
            {
                return ("--host missing", null);
            }
            if (ops.JobType == null)
            {
                return ("--job missing", null);
            }

            return (null, ops);
        }

        static void Main(string[] args)
        {
            var profile = new ProfileTimer();

            profile.StartPhase("all");

            var parseResult = ParseArgs(args);
            if (parseResult.Item1 != null)
            {
                PrintUsage((string)parseResult.Item1);
                Environment.Exit(1);
                return;
            }
            CommandLineOptions options = parseResult.Item2;

            profile.StartPhase("init logger");

            LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.Info);
            s_logger = NLog.LogManager.GetCurrentClassLogger();
            profile.EndPhaseAndLog();

            ThreadPool.SetMinThreads(4, 5);
            ThreadPool.SetMaxThreads(64, 10);

            int exitCode;
            try
            {
                profile.StartPhase("load cache meta file");
                CacheMetaManager.Ins.Load(options.CacheMetaInfoFile);
                profile.EndPhaseAndLog();
                profile.StartPhase("connect server");
                var conn = GenClient.Ins.Start(options.Host, options.Port, ProtocolStub.Factories);
                conn.Wait();
                profile.EndPhaseAndLog();

                profile.StartPhase("gen job");
                exitCode = SubmitGenJob(options);
                profile.EndPhaseAndLog();
            }
            catch (Exception e)
            {
                exitCode = 1;
                s_logger.Error(e);
            }

            CacheMetaManager.Ins.Save();
            profile.EndPhaseAndLog();
            if (exitCode == 0)
            {
                s_logger.Info("== succ ==");
            }
            else
            {
                s_logger.Error("== fail ==");
            }
            Environment.Exit(exitCode);
        }

        const int GEN_JOB_TIMEOUT = 120;

        private static int SubmitGenJob(CommandLineOptions options)
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
                }

                return 1;
            }

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
            return 0;
        }
    }
}
