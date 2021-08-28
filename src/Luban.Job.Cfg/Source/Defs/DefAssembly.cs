using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.l10n;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Defs
{
    public class TableDataInfo
    {
        public List<Record> MainRecords { get; }

        public List<Record> BranchRecords { get; }

        public List<Record> FinalRecords { get; set; }

        private List<Record> _notTestRecords;
        public List<Record> NotTestRecords
        {
            get
            {
                if (_notTestRecords == null)
                {
                    _notTestRecords = FinalRecords.Where(r => !r.IsTest).ToList();
                }
                return _notTestRecords;
            }
        }

        public Dictionary<DType, Record> FinalRecordMap { get; set; }

        public TableDataInfo(List<Record> mainRecords, List<Record> branchRecords)
        {
            MainRecords = mainRecords;
            BranchRecords = branchRecords;
        }
    }

    public class DefAssembly : DefAssemblyBase
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public new static DefAssembly LocalAssebmly { get => (DefAssembly)DefAssemblyBase.LocalAssebmly; set => DefAssemblyBase.LocalAssebmly = value; }

        public Service CfgTargetService { get; private set; }

        private readonly string _branchName;
        private readonly bool _exportTestData;

        public Branch TargetBranch { get; private set; }

        public TimeZoneInfo TimeZone { get; }

        public DefAssembly(string branchName, TimeZoneInfo timezone, bool exportTestData, RemoteAgent agent)
        {
            this._branchName = branchName;
            this.TimeZone = timezone;
            this._exportTestData = exportTestData;
            this.Agent = agent;
        }

        public bool NeedExport(List<string> groups)
        {
            if (groups.Count == 0)
            {
                return true;
            }
            return groups.Any(g => CfgTargetService.Groups.Contains(g));
        }

        private readonly List<Branch> _branches = new List<Branch>();

        private readonly List<Service> _cfgServices = new List<Service>();

        private readonly ConcurrentDictionary<string, TableDataInfo> _recordsByTables = new();

        public Dictionary<string, DefTable> CfgTables { get; } = new Dictionary<string, DefTable>();

        public RawTextTable RawTextTable { get; } = new RawTextTable();

        public TextTable ExportTextTable { get; private set; }

        public NotConvertTextSet NotConvertTextSet { get; private set; }

        public bool NeedL10nTextTranslate => ExportTextTable != null;

        public void InitL10n(string textValueFieldName)
        {
            ExportTextTable = new TextTable(this, textValueFieldName);
            NotConvertTextSet = new NotConvertTextSet();
        }

        public Branch GetBranch(string name)
        {
            return _branches.Find(b => b.Name == name);
        }

        public void AddCfgTable(DefTable table)
        {
            if (!CfgTables.TryAdd(table.FullName, table))
            {
                throw new Exception($"table:'{table.FullName}' duplicated");
            }
        }

        public DefTable GetCfgTable(string name)
        {
            return CfgTables.TryGetValue(name, out var t) ? t : null;
        }

        public void AddDataTable(DefTable table, List<Record> mainRecords, List<Record> branchRecords)
        {
            _recordsByTables[table.FullName] = new TableDataInfo(mainRecords, branchRecords);
        }

        public List<Record> GetTableAllDataList(DefTable table)
        {
            return _recordsByTables[table.FullName].FinalRecords;
        }

        public List<Record> GetTableExportDataList(DefTable table)
        {
            var tableDataInfo = _recordsByTables[table.FullName];
            return _exportTestData ? tableDataInfo.FinalRecords : tableDataInfo.NotTestRecords;
        }

        public TableDataInfo GetTableDataInfo(DefTable table)
        {
            return _recordsByTables[table.FullName];
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
                    throw new Exception($"service:'{targetService.Name}' ref:'{refType}' 类型不存在");
                }
                if (!refTypes.TryAdd(refType, this.Types[refType]))
                {
                    throw new Exception($"service:'{targetService.Name}' ref:'{refType}' 重复引用");
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

        public void Load(string outputService, Defines defines)
        {
            SupportDatetimeType = true;

            TopModule = defines.TopModule;

            CfgTargetService = defines.Services.Find(s => s.Name == outputService);

            if (CfgTargetService == null)
            {
                throw new ArgumentException($"service:{outputService} not exists");
            }

            if (!string.IsNullOrWhiteSpace(_branchName))
            {
                TargetBranch = defines.Branches.Find(b => b.Name == _branchName);
                if (TargetBranch == null)
                {
                    throw new Exception($"branch '{_branchName}' not in valid branch set");
                }
            }

            this._branches.AddRange(defines.Branches);

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
                    this.Agent.Error("precompile type:{0} error", type.FullName);
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
                    this.Agent.Error("compile type:{0} error", type.FullName);
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
                    this.Agent.Error("post compile type:{0} error", type.FullName);
                    s_logger.Error("post compile type:{0} error", type.FullName);
                    throw;
                }
            }

            // 丑陋. 怎么写更好？

            // 递归 设置DefBean及DefField 的 IsMultiRow

            MarkMultiRows();
        }

        public void MarkMultiRows()
        {
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
