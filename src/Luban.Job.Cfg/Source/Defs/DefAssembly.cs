using Luban.Config.Common.RawDefs;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.i10n;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Defs
{
    public class DefAssembly : DefAssemblyBase
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public Service CfgTargetService { get; private set; }

        public TimeZoneInfo TimeZone { get; }

        public DefAssembly(TimeZoneInfo timezone)
        {
            this.TimeZone = timezone;
        }

        public bool NeedExport(List<string> groups)
        {
            if (groups.Count == 0)
            {
                return true;
            }
            return groups.Any(g => CfgTargetService.Groups.Contains(g));
        }

        private readonly List<Service> _cfgServices = new List<Service>();

        private readonly ConcurrentDictionary<string, List<Record>> _recordsByTables = new();
        private readonly ConcurrentDictionary<string, Dictionary<DType, Record>> _recordsMapByTables = new();

        public Dictionary<string, DefTable> CfgTables { get; } = new Dictionary<string, DefTable>();

        public RawTextTable RawTextTable { get; } = new RawTextTable();

        public TextTable ExportTextTable { get; } = new TextTable();

        public NotConvertTextSet NotConvertTextSet { get; } = new NotConvertTextSet();

        public void AddCfgTable(DefTable table)
        {
            if (!CfgTables.TryAdd(table.FullName, table))
            {
                throw new Exception($"table:{table.FullName} duplicated");
            }
        }

        public DefTable GetCfgTable(string name)
        {
            return CfgTables.TryGetValue(name, out var t) ? t : null;
        }

        public void AddDataTable(DefTable table, List<Record> records)
        {
            _recordsByTables[table.FullName] = records;
        }

        public void SetDataTableMap(DefTable table, Dictionary<DType, Record> recordMap)
        {
            _recordsMapByTables[table.FullName] = recordMap;
        }

        public List<Record> GetTableDataList(DefTable table)
        {
            return _recordsByTables[table.FullName];
        }

        public Dictionary<DType, Record> GetTableDataMap(DefTable table)
        {
            return _recordsMapByTables[table.FullName];
        }

        public List<DefTable> GetExportTables()
        {
            return Types.Values.Where(t => t is DefTable ct && ct.NeedExport).Select(t => (DefTable)t).ToList();
        }

        public List<DefTypeBase> GetExportTypes()
        {
            var refTypes = new Dictionary<string, DefTypeBase>();
            var targetService = CfgTargetService;
            foreach (var refType in targetService.Refs)
            {
                if (!this.Types.ContainsKey(refType))
                {
                    throw new Exception($"service:{targetService.Name} ref:{refType} 类型不存在");
                }
                if (!refTypes.TryAdd(refType, this.Types[refType]))
                {
                    throw new Exception($"service:{targetService.Name} ref:{refType} 重复引用");
                }
            }
            foreach ((var fullTypeName, var type) in this.Types)
            {
                if (!refTypes.ContainsKey(fullTypeName) && (type is DefConst || type is DefEnum))
                {
                    refTypes.Add(fullTypeName, type);
                }
            }

            foreach (var table in GetExportTables())
            {
                refTypes[table.FullName] = table;
                table.ValueTType.Apply(RefTypeVisitor.Ins, refTypes);
            }

            return refTypes.Values.ToList();
        }

        public void Load(string outputService, Defines defines, RemoteAgent agent)
        {
            this.Agent = agent;
            SupportDatetimeType = true;

            TopModule = defines.TopModule;

            CfgTargetService = defines.Services.Find(s => s.Name == outputService);

            if (CfgTargetService == null)
            {
                throw new ArgumentException($"service:{outputService} not exists");
            }

            foreach (var c in defines.Consts)
            {
                AddType(new DefConst(c));
            }

            foreach (var e in defines.Enums)
            {
                AddType(new DefEnum(e));
            }

            foreach (var b in defines.Beans)
            {
                AddType(new DefBean((CfgBean)b));
            }

            foreach (var p in defines.Tables)
            {
                var table = new DefTable(p);
                AddType(table);
                AddCfgTable(table);
            }

            _cfgServices.AddRange(defines.Services);

            foreach (var type in Types.Values)
            {
                type.AssemblyBase = this;
            }

            foreach (var type in Types.Values)
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
            foreach (var type in Types.Values)
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
            foreach (var type in Types.Values)
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

            // 丑陋. 怎么写更好？

            // 递归 设置DefBean及DefField 的 IsMultiRow

            var multiRowBeans = new HashSet<DefBean>();
            for (bool anyMark = true; anyMark;)
            {
                anyMark = false;
                foreach (var type in this.Types.Values)
                {
                    if (type is DefBean beanType && !beanType.IsMultiRow)
                    {
                        bool isMultiRows;
                        if (beanType.IsNotAbstractType)
                        {
                            isMultiRows = beanType.HierarchyFields.Any(f => ((DefField)f).ComputeIsMultiRow());
                        }
                        else
                        {
                            isMultiRows = beanType.HierarchyNotAbstractChildren.Any(c => ((DefBean)c).IsMultiRow);
                        }
                        if (isMultiRows)
                        {
                            beanType.IsMultiRow = true;
                            //s_logger.Info("bean:{bean} is multi row", beanType.FullName);
                            anyMark = true;
                        }
                    }
                }

            }
        }
    }
}
