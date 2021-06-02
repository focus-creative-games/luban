using Luban.Common.Utils;
using Luban.Config.Common.RawDefs;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
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
                        var records = t.Assembly.GetTableDataList(t);
                        ValidateTableModeIndex(t, records);
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
                        var records = t.Assembly.GetTableDataList(t);
                        var visitor = new ValidatorVisitor(this);
                        try
                        {
                            CurrentVisitor = visitor;
                            visitor.ValidateTable(t, records);
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
                            agent.Error("记录 {0} = {1} (来自文件:{2}) 所引用文件:{3} 不存在", q.DataPath, q.Value, q.Source, string.Join(',', ls));
                        }
                        break;
                    }
                    default: throw new NotSupportedException();
                }
            }
        }

        private void ValidateTableModeIndex(DefTable table, List<DType> records)
        {
            var recordMap = new Dictionary<DType, DBean>();

            switch (table.Mode)
            {
                case ETableMode.ONE:
                {
                    if (records.Count != 1)
                    {
                        throw new Exception($"配置表 {table.FullName} 是单值表 mode=one,但数据个数:{records.Count} != 1");
                    }
                    break;
                }
                case ETableMode.MAP:
                {
                    foreach (DBean r in records)
                    {
                        DType key = r.Fields[table.IndexFieldIdIndex];
                        if (!recordMap.TryAdd(key, r))
                        {
                            throw new Exception($@"配置表 {table.FullName} 主键字段:{table.Index} 主键值:{key} 重复.
        记录1 来自文件:{DataUtil.GetSourceFile(r)}
        记录2 来自文件:{DataUtil.GetSourceFile(recordMap[key])}
");
                        }

                    }
                    break;
                }
                case ETableMode.BMAP:
                {
                    var twoKeyMap = new Dictionary<(DType, DType), DBean>();
                    foreach (DBean r in records)
                    {
                        DType key1 = r.Fields[table.IndexFieldIdIndex1];
                        DType key2 = r.Fields[table.IndexFieldIdIndex2];
                        if (!twoKeyMap.TryAdd((key1, key2), r))
                        {
                            throw new Exception($@"配置表 {table.FullName} 主键字段:{table.Index} 主键值:({key1},{key2})重复. 
        记录1 来自文件:{DataUtil.GetSourceFile(r)}
        记录2 来自文件:{DataUtil.GetSourceFile(twoKeyMap[(key1, key2)])}
");
                        }
                        // 目前不支持 双key索引检查,但支持主key索引检查.
                        // 所以至少塞入一个,让ref检查能通过
                        recordMap[key1] = r;
                    }
                    break;
                }
            }
            table.Assembly.SetDataTableMap(table, recordMap);
        }
    }
}
