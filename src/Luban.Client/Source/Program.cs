using Luban.Client.Common.Net;
using Luban.Client.Common.Utils;
using Luban.Common.Protos;
using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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

            public string WatchDir { get; set; }
        }

        private static NLog.Logger s_logger;

        private static FileSystemWatcher s_watcher;

        private static void PrintUsage(string err)
        {
            Console.WriteLine("ERRORS:");
            Console.WriteLine("\t" + err);
            Console.WriteLine(@"
Luban.Client <Options>...  [-- [job options]]
e.g.
    Luban.Client -h 127.0.0.1 -p 2234 -j cfg --  --name abc

Options:
  -h, --host  <host>            Required. host ip
  -p  --port  <port>            port. default 8899
  -j  --job   <job>             Required. job type.  avaliable value: cfg
  -v  --verbose                 verbose print
  -c  --cachemetafile <file>    cache meta file name. default is '.cache.meta'
  -w  --watch  <dir>            watch data change and regenerate.
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
                        case "-w":
                        case "--watch":
                        {
                            ops.WatchDir = args[++i];
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


            if (string.IsNullOrWhiteSpace(options.WatchDir))
            {
                Environment.Exit(GenOnce(options, profile));
                profile.EndPhaseAndLog();
            }
            else
            {
                GenOnce(options, profile);
                var watcher = new FileSystemWatcher(options.WatchDir);

                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                watcher.Changed += (o, p) => OnWatchChange(options, profile);
                watcher.Created += (o, p) => OnWatchChange(options, profile);
                watcher.Deleted += (o, p) => OnWatchChange(options, profile);
                watcher.Renamed += (o, p) => OnWatchChange(options, profile);

                //watcher.Filter = "*.txt";
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                s_logger.Info("=== start watch. dir:{} ==", options.WatchDir);
                s_watcher = watcher;
            }
        }

        private static readonly object s_watchLocker = new object();
        private static bool s_watchDirChange = false;

        private static void OnWatchChange(CommandLineOptions options, ProfileTimer profile)
        {
            lock (s_watchLocker)
            {
                if (s_watchDirChange)
                {
                    return;
                }
                s_watchDirChange = true;

                Task.Run(async () =>
                {
                    s_logger.Info("=== start new generation ==");
                    await Task.Delay(200);

                    lock (s_watchLocker)
                    {
                        s_watchDirChange = false;
                        GenOnce(options, profile);
                    }
                    s_logger.Info("=== watch changes ==");
                });
            }

        }

        private static int GenOnce(CommandLineOptions options, ProfileTimer profile)
        {
            int exitCode;
            try
            {

                profile.StartPhase("connect server");
                var conn = GenClient.Start(options.Host, options.Port, ProtocolStub.Factories);

                profile.StartPhase("load cache meta file");
                CacheMetaManager.Ins.Load(options.CacheMetaInfoFile);
                profile.EndPhaseAndLog();

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
