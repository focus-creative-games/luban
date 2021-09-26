using Bright.Net;
using Bright.Net.Bootstraps;
using Bright.Net.Channels;
using Bright.Net.Codecs;
using Bright.Net.ServiceModes.Managers;
using Bright.Time;
using Luban.Client.Common.Utils;
using Luban.Common.Protos;
using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Luban.Client.Common.Net
{
    public class Session : SessionBase
    {
        public override void OnActive()
        {

        }

        public override void OnInactive()
        {
        }
    }

    public class GenClient : ClientManager<Session>
    {

        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public static GenClient Ins { get; private set; }

        public static async Task Start(string host, int port, Dictionary<int, ProtocolCreator> factories)
        {
            Ins = new GenClient();
            var c = new TcpClientBootstrap
            {
                RemoteAddress = new IPEndPoint(IPAddress.Parse(host), port),
                ConnectTimeoutMills = 3000,
                EventLoop = new EventLoop(null),
                InitHandler = ch =>
                {
                    ch.Pipeline.AddLast(new ProtocolFrameCodec(20_000_000, new RecycleByteBufPool(100, 10), new DefaultProtocolAllocator(factories)));
                    ch.Pipeline.AddLast(Ins);
                }
            };

            var ch = await c.ConnectAsync().ConfigureAwait(false);
        }

        public static void Stop()
        {
            _ = Ins.Session.CloseAsync();
            Ins = null;
        }

        public override void HandleProtocol(Protocol proto)
        {

            switch (proto.GetTypeId())
            {
                case GetInputFile.ID:
                {
                    Task.Run(() => _ = OnGetInputFileAsync((GetInputFile)proto));
                    break;
                }
                case GetImportFileOrDirectory.ID:
                {
                    Task.Run(() => _ = OnGetImportFileOrDirectoryAsync((GetImportFileOrDirectory)proto));
                    break;
                }
                case QueryFilesExists.ID:
                {
                    Task.Run(() => Process((QueryFilesExists)proto));
                    break;
                }
                case PushLog.ID:
                {
                    Process((PushLog)proto);
                    break;
                }
                case PushException.ID:
                {
                    Process((PushException)proto);
                    break;
                }
                default:
                {
                    s_logger.Error("unknown proto:{proto}", proto);
                    break;
                }
            }
        }

        private async Task OnGetImportFileOrDirectoryAsync(GetImportFileOrDirectory rpc)
        {
            long t1 = TimeUtil.NowMillis;
            var file = rpc.Arg.FileOrDirName;
            var suffixes = rpc.Arg.InclusiveSuffixs;
            var re = new GetImportFileOrDirectoryRes()
            {
                SubFiles = new List<Luban.Common.Protos.FileInfo>(),
            };

            try
            {
                if (Directory.Exists(file))
                {
                    re.Err = 0;
                    re.IsFile = false;
                    foreach (var subFile in Directory.GetFiles(file, "*", SearchOption.AllDirectories))
                    {
                        if (FileUtil.IsValidInputFile(subFile) && (suffixes.Count == 0 || suffixes.Any(s => subFile.EndsWith(s))))
                        {
                            var md5 = await CacheMetaManager.Ins.GetOrUpdateFileMd5Async(subFile);
                            re.SubFiles.Add(new Luban.Common.Protos.FileInfo() { FilePath = FileUtil.Standardize(subFile), MD5 = md5 });
                        }
                    }

                }
                else if (File.Exists(file))
                {
                    re.IsFile = true;
                    re.Md5 = await CacheMetaManager.Ins.GetOrUpdateFileMd5Async(file);
                }
                else
                {
                    re.Err = Luban.Common.EErrorCode.FILE_OR_DIR_NOT_EXISTS;
                }
            }
            catch (Exception e)
            {
                re.Err = Luban.Common.EErrorCode.READ_FILE_FAIL;
                s_logger.Error(e);
            }

            s_logger.Trace(" GetImportFileOrDirectory file:{file} err:{err} cost:{time}", file, re.Err, TimeUtil.NowMillis - t1);

            Session.ReplyRpc<GetImportFileOrDirectory, GetImportFileOrDirectoryArg, GetImportFileOrDirectoryRes>(rpc, re);
        }

        private async Task OnGetInputFileAsync(GetInputFile rpc)
        {
            long t1 = TimeUtil.NowMillis;
            var res = new GetInputFileRes() { Err = Luban.Common.EErrorCode.OK };
            try
            {
                res.Content = await FileUtil.ReadAllBytesAsync(rpc.Arg.File);
                //res.Content = FileUtil.ReadAllBytes(rpc.Arg.File);
                res.Err = Luban.Common.EErrorCode.OK;
            }
            catch (Exception)
            {
                res.Err = Luban.Common.EErrorCode.READ_FILE_FAIL;
            }
            s_logger.Info(" get input file:{file} err:{err} cost:{time}", rpc.Arg.File, res.Err, TimeUtil.NowMillis - t1);

            Session.ReplyRpc<GetInputFile, GetInputFileArg, GetInputFileRes>(rpc, res);
        }

        private void Process(QueryFilesExists p)
        {
            var root = p.Arg.Root;
            var files = p.Arg.Files;
            var re = new QueryFilesExistsRes() { Exists = new List<bool>(files.Count) };
            foreach (var f in files)
            {
                re.Exists.Add(File.Exists(Path.Combine(root, f)));
            }
            Session.ReplyRpc<QueryFilesExists, QueryFilesExistsArg, QueryFilesExistsRes>(p, re);
        }

        private void Process(PushLog p)
        {
            switch (p.Level)
            {
                case "trace":
                {
                    s_logger.Trace(p.LogContent);
                    break;
                }
                case "info":
                {
                    s_logger.Info(p.LogContent);
                    break;
                }
                default:
                {
                    s_logger.Error(p.LogContent);
                    break;
                }

            }
        }

        private void Process(PushException p)
        {
            s_logger.Error(p.LogContent);
            s_logger.Error(p.StackTrace);
        }
    }
}
