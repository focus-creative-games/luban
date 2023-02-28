using Bright.Collections;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.l10n;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
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

        public static new DefAssembly LocalAssebmly { get => (DefAssembly)DefAssemblyBase.LocalAssebmly; set => DefAssemblyBase.LocalAssebmly = value; }

        public Service CfgTargetService { get; private set; }

        private readonly string _patchName;

        private readonly List<string> _excludeTags;

        public Patch TargetPatch { get; private set; }

        public TimeZoneInfo TimeZone { get; }

        public bool OutputCompactJson { get; set; }

        public string TableManagerName => CfgTargetService.Manager;

        public List<string> ExcludeTags => _excludeTags;

        public DefAssembly(string patchName, TimeZoneInfo timezone, List<string> excludeTags, IAgent agent)
        {
            this._patchName = patchName;
            this.TimeZone = timezone;
            this._excludeTags = excludeTags;
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

        private readonly List<Patch> _patches = new List<Patch>();

        private readonly List<Service> _cfgServices = new List<Service>();

        private readonly Dictionary<string, DefRefGroup> _refGroups = new();

        private readonly ConcurrentDictionary<string, TableDataInfo> _recordsByTables = new();

        public Dictionary<string, DefTable> CfgTablesByName { get; } = new();

        public Dictionary<string, DefTable> CfgTablesByFullName { get; } = new Dictionary<string, DefTable>();

        public RawTextTable RawTextTable { get; } = new RawTextTable();

        public TextTable ExportTextTable { get; private set; }

        public NotConvertTextSet NotConvertTextSet { get; private set; }

        public bool NeedL10nTextTranslate => ExportTextTable != null;

        private HashSet<string> _overrideOutputTables;

        private readonly HashSet<string> _outputIncludeTables = new();

        private readonly HashSet<string> _outputExcludeTables = new();

        public void InitL10n(string textValueFieldName)
        {
            ExportTextTable = new TextTable(this, textValueFieldName);
            NotConvertTextSet = new NotConvertTextSet();
        }

        public Patch GetPatch(string name)
        {
            return _patches.Find(b => b.Name == name);
        }

        public void AddCfgTable(DefTable table)
        {
            if (!CfgTablesByFullName.TryAdd(table.FullName, table))
            {
                throw new Exception($"table:'{table.FullName}' duplicated");
            }
            if (!CfgTablesByName.TryAdd(table.Name, table))
            {
                throw new Exception($"table:'{table.FullName} 与 table:'{CfgTablesByName[table.Name].FullName}' 的表名重复(不同模块下也不允许定义同名表，将来可能会放开限制)");
            }
        }

        public DefTable GetCfgTable(string name)
        {
            return CfgTablesByFullName.TryGetValue(name, out var t) ? t : null;
        }

        public void AddDataTable(DefTable table, List<Record> mainRecords, List<Record> patchRecords)
        {
            _recordsByTables[table.FullName] = new TableDataInfo(table, mainRecords, patchRecords);
        }

        public List<Record> GetTableAllDataList(DefTable table)
        {
            return _recordsByTables[table.FullName].FinalRecords;
        }

        public List<Record> GetTableExportDataList(DefTable table)
        {
            var tableDataInfo = _recordsByTables[table.FullName];
            if (_excludeTags.Count == 0)
            {
                return tableDataInfo.FinalRecords;
            }
            else
            {
                var finalRecords = tableDataInfo.FinalRecords.Where(r => r.IsNotFiltered(_excludeTags)).ToList();
                if (table.IsOneValueTable && finalRecords.Count != 1)
                {
                    throw new Exception($"配置表 {table.FullName} 是单值表 mode=one,但数据个数:{finalRecords.Count} != 1");
                }
                return finalRecords;
            }
        }

        public static List<Record> ToSortByKeyDataList(DefTable table, List<Record> originRecords)
        {
            var sortedRecords = new List<Record>(originRecords);

            DefField keyField = table.IndexField;
            if (keyField != null && (keyField.CType is TInt || keyField.CType is TLong))
            {
                string keyFieldName = keyField.Name;
                sortedRecords.Sort((a, b) =>
                {
                    DType keya = a.Data.GetField(keyFieldName);
                    DType keyb = b.Data.GetField(keyFieldName);
                    switch (keya)
                    {
                        case DInt ai: return ai.Value.CompareTo((keyb as DInt).Value);
                        case DLong al: return al.Value.CompareTo((keyb as DLong).Value);
                        default: throw new NotSupportedException();
                    }
                });
            }
            return sortedRecords;
        }

        public TableDataInfo GetTableDataInfo(DefTable table)
        {
            return _recordsByTables[table.FullName];
        }

        public List<DefTable> GetAllTables()
        {
            return TypeList.Where(t => t is DefTable).Cast<DefTable>().ToList();
        }

        public List<DefTable> GetExportTables()
        {
            return TypeList.Where(t => t is DefTable ct
            && !_outputExcludeTables.Contains(t.FullName)
            && (_outputIncludeTables.Contains(t.FullName) || (_overrideOutputTables == null ? ct.NeedExport : _overrideOutputTables.Contains(ct.FullName)))
            ).Select(t => (DefTable)t).ToList();
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
            foreach (var e in this.Types)
            {
                if (!refTypes.ContainsKey(e.Key) && (e.Value is DefEnum))
                {
                    refTypes.Add(e.Key, e.Value);
                }
            }

            foreach (var table in GetExportTables())
            {
                refTypes[table.FullName] = table;
                table.ValueTType.Apply(RefTypeVisitor.Ins, refTypes);
            }

            return refTypes.Values.ToList();
        }

        private void AddRefGroup(RefGroup g)
        {
            if (_refGroups.ContainsKey(g.Name))
            {
                throw new Exception($"refgroup:{g.Name} 重复");
            }
            _refGroups.Add(g.Name, new DefRefGroup(g));
        }

        public DefRefGroup GetRefGroup(string groupName)
        {
            return _refGroups.TryGetValue(groupName, out var refGroup) ? refGroup : null;
        }

        private IEnumerable<string> SplitTableList(string tables)
        {
            return tables.Split(',').Select(t => t.Trim());
        }

        public void Load(Defines defines, IAgent agent, GenArgs args)
        {
            LoadCommon(defines, agent, args);

            OutputCompactJson = args.OutputDataCompactJson;

            SupportDatetimeType = true;

            CfgTargetService = defines.Services.Find(s => s.Name == args.Service);

            if (CfgTargetService == null)
            {
                throw new ArgumentException($"service:{args.Service} not exists");
            }

            if (!string.IsNullOrWhiteSpace(_patchName))
            {
                TargetPatch = defines.Patches.Find(b => b.Name == _patchName);
                if (TargetPatch == null)
                {
                    throw new Exception($"patch '{_patchName}' not in valid patch set");
                }
            }

            this._patches.AddRange(defines.Patches);

            foreach (var g in defines.RefGroups)
            {
                AddRefGroup(g);
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

            if (!string.IsNullOrWhiteSpace(args.OutputTables))
            {
                foreach (var tableFullName in SplitTableList(args.OutputTables))
                {
                    if (GetCfgTable(tableFullName) == null)
                    {
                        throw new Exception($"--output:tables 参数中 table:'{tableFullName}' 不存在");
                    }
                    _overrideOutputTables ??= new HashSet<string>();
                    _overrideOutputTables.Add(tableFullName);
                }
            }
            if (!string.IsNullOrWhiteSpace(args.OutputIncludeTables))
            {
                foreach (var tableFullName in SplitTableList(args.OutputIncludeTables))
                {
                    if (GetCfgTable(tableFullName) == null)
                    {
                        throw new Exception($"--output:include_tables 参数中 table:'{tableFullName}' 不存在");
                    }
                    _outputIncludeTables.Add(tableFullName);
                }
            }
            if (!string.IsNullOrWhiteSpace(args.OutputExcludeTables))
            {
                foreach (var tableFullName in SplitTableList(args.OutputExcludeTables))
                {
                    if (GetCfgTable(tableFullName) == null)
                    {
                        throw new Exception($"--output:exclude_tables 参数中 table:'{tableFullName}' 不存在");
                    }
                    _outputExcludeTables.Add(tableFullName);
                }
            }

            _cfgServices.AddRange(defines.Services);

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
                    this.Agent.Error("precompile type:{0} error", type.FullName);
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
                    this.Agent.Error("compile type:{0} error", type.FullName);
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
                    this.Agent.Error("post compile type:{0} error", type.FullName);
                    s_logger.Error("post compile type:{0} error", type.FullName);
                    throw;
                }
            }

            foreach (var externalType in defines.ExternalTypes.Values)
            {
                AddExternalType(externalType);
            }
        }
    }
}
