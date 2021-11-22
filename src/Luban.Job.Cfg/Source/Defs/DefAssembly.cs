using Bright.Collections;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
#if !LUBAN_LITE
using Luban.Job.Cfg.l10n;
#endif
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
    public class TableDataInfo
    {
        public DefTable Table { get; }

        public List<Record> MainRecords { get; }

        public List<Record> PatchRecords { get; }

        public List<Record> FinalRecords { get; set; }

        public Dictionary<DType, Record> FinalRecordMap { get; set; }

        public TableDataInfo(DefTable table, List<Record> mainRecords, List<Record> patchRecords)
        {
            Table = table;
            MainRecords = mainRecords;
            PatchRecords = patchRecords;
        }
    }

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

        private readonly ConcurrentDictionary<string, TableDataInfo> _recordsByTables = new();

        public Dictionary<string, DefTable> CfgTables { get; } = new Dictionary<string, DefTable>();

#if !LUBAN_LITE
        public RawTextTable RawTextTable { get; } = new RawTextTable();

        public TextTable ExportTextTable { get; private set; }

        public NotConvertTextSet NotConvertTextSet { get; private set; }

        public bool NeedL10nTextTranslate => ExportTextTable != null;

        public void InitL10n(string textValueFieldName)
        {
            ExportTextTable = new TextTable(this, textValueFieldName);
            NotConvertTextSet = new NotConvertTextSet();
        }

#endif

        public Patch GetPatch(string name)
        {
            return _patches.Find(b => b.Name == name);
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

        public void Load(string outputService, Defines defines)
        {
            SupportDatetimeType = true;

            TopModule = defines.TopModule;

            CfgTargetService = defines.Services.Find(s => s.Name == outputService);

            if (CfgTargetService == null)
            {
                throw new ArgumentException($"service:{outputService} not exists");
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
        }
    }
}
