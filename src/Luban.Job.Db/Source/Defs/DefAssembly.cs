using Luban.Job.Common.Defs;
using Luban.Job.Db.RawDefs;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Db.Defs
{
    class DefAssembly : DefAssemblyBase
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, DefTypeBase> name2DbTables = new Dictionary<string, DefTypeBase>();

        private readonly Dictionary<int, DefTable> id2DbTables = new Dictionary<int, DefTable>();

        public void AddDbTable(DefTable table)
        {
            if (!name2DbTables.TryAdd(table.FullName, table))
            {
                throw new Exception($"table:'{table.FullName}' duplicated");
            }
            if (!id2DbTables.TryAdd(table.TableUId, table))
            {
                throw new Exception($"table:'{table.FullName}' 与 table:'{id2DbTables[table.TableUId].FullName}' id:{table.TableUId} duplicated");
            }
        }


        public void Load(Defines defines, RemoteAgent agent, GenArgs args)
        {
            LoadCommon(defines, agent, args);
            this.SupportNullable = false;

            foreach (var e in defines.Enums)
            {
                AddType(new DefEnum(e));
            }

            foreach (var b in defines.Beans)
            {
                AddType(new DefBean(b));
            }

            foreach (var p in defines.DbTables)
            {
                AddType(new DefTable(p));
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
