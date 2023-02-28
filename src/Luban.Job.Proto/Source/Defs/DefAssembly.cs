using Luban.Job.Common.Defs;
using Luban.Job.Proto.RawDefs;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Proto.Defs
{

    class DefAssembly : DefAssemblyBase
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<int, DefTypeBase> id2Protos = new Dictionary<int, DefTypeBase>();

        public DefTypeBase GetProto(int id)
        {
            return id2Protos.TryGetValue(id, out var p) ? p : null;
        }

        public void AddProto(DefTypeBase proto)
        {
            if (!id2Protos.TryAdd(proto.Id, proto))
            {
                throw new Exception($"proto:'{proto.FullName}' id:{proto.Id} duplicated with '{id2Protos[proto.Id].FullName}'");
            }
        }

        public void Load(Defines defines, RemoteAgent agent, GenArgs args)
        {
            LoadCommon(defines, agent, args);

            foreach (var e in defines.Enums)
            {
                AddType(new DefEnum(e));
            }

            foreach (var b in defines.Beans)
            {
                AddType(new DefBean(b));
            }

            foreach (var p in defines.Protos)
            {
                AddType(new DefProto(p));
            }
            foreach (var r in defines.Rpcs)
            {
                AddType(new DefRpc(r));
            }

            foreach (var type in TypeList)
            {
                type.AssemblyBase = this;
            }

            foreach (var type in TypeList)
            {
                try
                {
                    s_logger.Trace("precompile type:{0} begin", type.FullName);
                    type.PreCompile();
                    s_logger.Trace("precompile type:{0} end", type.FullName);
                }
                catch (Exception)
                {
                    agent.Error("precompile type:{0} error", type.FullName);
                    throw;
                }
            }
            foreach (var type in TypeList)
            {
                try
                {
                    s_logger.Trace("compile type:{0} begin", type.FullName);
                    type.Compile();
                    s_logger.Trace("compile type:{0} end", type.FullName);
                }
                catch (Exception)
                {
                    agent.Error("compile type:{0} error", type.FullName);
                    s_logger.Error("compile type:{0} error", type.FullName);
                    throw;
                }
            }
            foreach (var type in TypeList)
            {
                try
                {
                    s_logger.Trace("post compile type:{0} begin", type.FullName);
                    type.PostCompile();
                    s_logger.Trace("post compile type:{0} end", type.FullName);
                }
                catch (Exception)
                {
                    agent.Error("post compile type:{0} error", type.FullName);
                    s_logger.Error("post compile type:{0} error", type.FullName);
                    throw;
                }
            }

        }

        public List<DefTypeBase> GetExportTypes()
        {
            return TypeList;
        }
    }
}
