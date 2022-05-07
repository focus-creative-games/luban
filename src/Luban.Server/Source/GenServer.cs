using Bright.Net;
using Bright.Net.Bootstraps;
using Bright.Net.Channels;
using Bright.Net.Codecs;
using Bright.Net.ServiceModes.Managers;
using Luban.Common;
using Luban.Common.Protos;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Net;

namespace Luban.Server
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

    public class GenServer : ServerManager<Session>
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public static GenServer Ins { get; } = new GenServer();

        private readonly Dictionary<int, Action<Session, Protocol>> _handlers = new Dictionary<int, Action<Session, Protocol>>();

        private readonly Dictionary<string, IJobController> _jobs = new Dictionary<string, IJobController>();

        private bool _localServer;

        public void Start(bool localhost, int port, Dictionary<int, ProtocolCreator> factories)
        {
            _localServer = localhost;
            _handlers.Add(GetOutputFile.ID, (s, p) => OnGetOutputFile(s, (GetOutputFile)p));
            _handlers.Add(GenJob.ID, (s, p) => OnGenJob(s, (GenJob)p));

            var worker = new EventLoopGroup(4, 16);
            var server = new TcpServerBootstrap
            {
                LocalAddress = new IPEndPoint(localhost ? IPAddress.Loopback : IPAddress.Any, port),

                ChildrenEventLoopGroup = worker,
                EventLoop = worker.ChooseEventLoop(),

                InitChildrenHandler = (s, c) =>
                {
                    c.Pipeline.AddLast(new ProtocolFrameCodec(100 * 1024 * 1024, new RecycleByteBufPool(100, 100), new DefaultProtocolAllocator(factories)));
                    c.Pipeline.AddLast(this);
                }
            };

            _ = server.ListenAsync();
        }

        public void RegisterJob(string jobType, IJobController jobController)
        {
            s_logger.Debug("register job. name:{name} class:{class}", jobType, jobController.GetType().FullName);
            _jobs.Add(jobType, jobController);
        }

        protected override void OnAddSession(Session s)
        {

        }

        protected override void OnRemoveSession(Session s)
        {

        }

        protected override void HandleProtocol(Session session, Protocol proto)
        {
            s_logger.Trace("session:{id} protocol:{@proto}", session.Id, proto);
            if (_handlers.TryGetValue(proto.GetTypeId(), out var handler))
            {
                handler(session, proto);
            }
            else
            {
                s_logger.Error("unknown proto:{proto}", proto);
            }
        }

        private void OnGetOutputFile(Session session, GetOutputFile rpc)
        {
            var cache = CacheManager.Ins.FindCache(rpc.Arg.MD5);
            session.ReplyRpc<GetOutputFile, GetOutputFileArg, GetOutputFileRes>(rpc, new GetOutputFileRes()
            {
                Exists = cache != null,
                FileContent = cache?.Content,
            });
        }


        private void OnGenJob(Session session, GenJob rpc)
        {
            s_logger.Info("onGenJob. arg:{@arg}", rpc.Arg);

            if (_jobs.TryGetValue(rpc.Arg.JobType, out var jobController))
            {
                _ = jobController.GenAsync(_localServer ? new LocalAgent(session) : new RemoteAgent(session), rpc);
            }
            else
            {
                session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, new GenJobRes()
                {
                    ErrCode = EErrorCode.UNKNOWN_JOB_TYPE,
                    ErrMsg = "unknown job type",
                    FileGroups = new List<FileGroup>(),
                });
            }
        }
    }
}
