using Bright.Collections;
using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Cfg
{
    public class PathQuery
    {
        public PathValidator Validator { get; set; }

        public string DataPath { get; set; }

        public string Value { get; set; }

        public string Source { get; set; }

        public object QueryPath { get; set; }
    }

    public class ValidatorContext
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        [ThreadStatic]
        private static ValidatorVisitor t_visitor;

        public static ValidatorVisitor CurrentVisitor { get => t_visitor; set => t_visitor = value; }

        public static string CurrentRecordPath => TypeUtil.MakeFullName(CurrentVisitor.Path);

        public DefAssembly Assembly { get; }

        public string RootDir { get; }

        private readonly List<PathQuery> _pathQuerys = new List<PathQuery>(1000);

        public void AddPathQuery(PathQuery query)
        {
            lock (this)
            {
                _pathQuerys.Add(query);
            }
        }

        public List<PathQuery> GetPathQueries() => _pathQuerys;

        public ValidatorContext(DefAssembly ass, string rootDir)
        {
            this.Assembly = ass;
            this.RootDir = rootDir;
        }

        public async Task ValidateTables(IEnumerable<DefTable> tables)
        {
            {
                var tasks = new List<Task>();
                foreach (var t in tables)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        ValidateTableModeIndex(t);
                    }));
                }
                await Task.WhenAll(tasks);
            }

            {
                var tasks = new List<Task>();
                foreach (var t in tables)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        var records = t.Assembly.GetTableAllDataList(t);
                        var visitor = new ValidatorVisitor(this);
                        try
                        {
                            CurrentVisitor = visitor;
                            visitor.ValidateTable(t, records);
#if !LUBAN_LITE
                            if (this.Assembly.NeedL10nTextTranslate)
                            {
                                ValidateText(t, records);
                            }
#endif
                        }
                        finally
                        {
                            CurrentVisitor = null;
                        }
                    }));
                }
                await Task.WhenAll(tasks);
            }

            if (!string.IsNullOrWhiteSpace(RootDir))
            {
                await ValidatePaths();
            }
        }

#if !LUBAN_LITE
        private void ValidateText(DefTable table, List<Record> records)
        {
            foreach (var r in records)
            {
                CurrentVisitor.CurrentValidateRecord = r;
                r.Data.Apply(TextValidatorVisitor.Ins, this.Assembly.RawTextTable);
            }
            CurrentVisitor.CurrentValidateRecord = null;
        }
#endif

        private async Task ValidatePaths()
        {
            var queryFiles = new HashSet<string>(_pathQuerys.Count * 2);
            foreach (var q in _pathQuerys)
            {
                switch (q.QueryPath)
                {
                    case string s: queryFiles.Add(s); break;
                    case List<string> ls:
                    {
                        foreach (var p in ls)
                        {
                            queryFiles.Add(p);
                        }
                        break;
                    }
                    default: throw new NotSupportedException();
                }
            }

            var files = new List<string>(queryFiles);
            var res = await this.Assembly.Agent.QueryFileExistsAsync(new Luban.Common.Protos.QueryFilesExistsArg() { Root = RootDir, Files = files });
            var fileNotExistsSet = new HashSet<string>(queryFiles.Count * 2);
            for (int i = 0; i < files.Count; i++)
            {
                if (!res.Exists[i])
                {
                    fileNotExistsSet.Add(files[i]);
                }
            }

            var agent = this.Assembly.Agent;

            foreach (var q in _pathQuerys)
            {
                switch (q.QueryPath)
                {
                    case string s:
                    {
                        if (fileNotExistsSet.Contains(s))
                        {
                            agent.Error("记录 {0} = {1} (来自文件:{2}) 所引用文件:{3} 不存在", q.DataPath, q.Value, q.Source, s);
                        }
                        break;
                    }
                    case List<string> ls:
                    {
                        if (ls.All(f => fileNotExistsSet.Contains(f)))
                        {
#if !LUBAN_LITE
                            agent.Error("记录 {0} = {1} (来自文件:{2}) 所引用文件:{3} 不存在", q.DataPath, q.Value, q.Source, string.Join(',', ls));
#else
                            agent.Error("记录 {0} = {1} (来自文件:{2}) 所引用文件:{3} 不存在", q.DataPath, q.Value, q.Source, string.Join(",", ls));
#endif
                        }
                        break;
                    }
                    default: throw new NotSupportedException();
                }
            }
        }

        private void ValidateTableModeIndex(DefTable table)
        {
            var tableDataInfo = Assembly.GetTableDataInfo(table);

            List<Record> mainRecords = tableDataInfo.MainRecords;
            List<Record> patchRecords = tableDataInfo.PatchRecords;

            // 这么大费周张是为了保证被覆盖的id仍然保持原来的顺序，而不是出现在最后
            int index = 0;
            foreach (var r in mainRecords)
            {
                r.Index = index++;
            }
            if (patchRecords != null)
            {
                foreach (var r in patchRecords)
                {
                    r.Index = index++;
                }
            }

            var mainRecordMap = new Dictionary<DType, Record>();

            switch (table.Mode)
            {
                case ETableMode.ONE:
                {
                    //if (mainRecords.Count != 1)
                    //{
                    //    throw new Exception($"配置表 {table.FullName} 是单值表 mode=one,但主文件数据个数:{mainRecords.Count} != 1");
                    //}
                    //if (patchRecords != null && patchRecords.Count != 1)
                    //{
                    //    throw new Exception($"配置表 {table.FullName} 是单值表 mode=one,但分支文件数据个数:{patchRecords.Count} != 1");
                    //}
                    if (patchRecords != null)
                    {
                        mainRecords = patchRecords;
                    }
                    break;
                }
                case ETableMode.MAP:
                {
                    foreach (Record r in mainRecords)
                    {
                        DType key = r.Data.Fields[table.IndexFieldIdIndex];
                        if (!mainRecordMap.TryAdd(key, r))
                        {
                            throw new Exception($@"配置表 '{table.FullName}' 主文件 主键字段:'{table.Index}' 主键值:'{key}' 重复.
        记录1 来自文件:{r.Source}
        记录2 来自文件:{mainRecordMap[key].Source}
");
                        }
                    }
                    if (patchRecords != null)
                    {
                        var patchRecordMap = new Dictionary<DType, Record>();
                        foreach (Record r in patchRecords)
                        {
                            DType key = r.Data.Fields[table.IndexFieldIdIndex];
                            if (!patchRecordMap.TryAdd(key, r))
                            {
                                throw new Exception($@"配置表 '{table.FullName}' 分支文件 主键字段:'{table.Index}' 主键值:'{key}' 重复.
        记录1 来自文件:{r.Source}
        记录2 来自文件:{patchRecordMap[key].Source}
");
                            }
                            if (mainRecordMap.TryGetValue(key, out var old))
                            {
                                s_logger.Debug("配置表 {} 分支文件 主键:{} 覆盖 主文件记录", table.FullName, key);
                                mainRecords[old.Index] = r;
                            }
                            mainRecordMap[key] = r;
                        }
                    }
                    break;
                }
            }
#if !LUBAN_LITE
            tableDataInfo.FinalRecords = mainRecords;
            tableDataInfo.FinalRecordMap = mainRecordMap;
#endif
        }
    }
}
