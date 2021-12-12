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
                        if (this.Assembly.NeedL10nTextTranslate)
                        {
                            ValidateText(t, records);
                        }
                    }
                    finally
                    {
                        CurrentVisitor = null;
                    }
                }));
            }
            await Task.WhenAll(tasks);

            if (!string.IsNullOrWhiteSpace(RootDir))
            {
                await ValidatePaths();
            }
        }

        private void ValidateText(DefTable table, List<Record> records)
        {
            foreach (var r in records)
            {
                CurrentVisitor.CurrentValidateRecord = r;
                r.Data.Apply(TextValidatorVisitor.Ins, this.Assembly.RawTextTable);
            }
            CurrentVisitor.CurrentValidateRecord = null;
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

    }
}
