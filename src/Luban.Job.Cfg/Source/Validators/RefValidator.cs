using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Validators
{
    public class RefValidator : IValidator
    {
        public const string NAME = "ref";

        public List<string> Tables { get; }

        public string FirstTable => GetActualTableName(Tables[0]);

        public static string GetActualTableName(string table)
        {
#if !LUBAN_ASSISTANT
            return table.EndsWith("?") ? table[0..^1] : table;
#else
            return table.EndsWith("?") ? table.Substring(0, table.Length - 1) : table;
#endif
        }

        public RefValidator(List<string> tables)
        {
            this.Tables = new List<string>(tables);
        }

        public void Validate(ValidatorContext ctx, DType key, bool nullable)
        {
            // 对于可空字段，跳过检查
            if (nullable && key == null)
            {
                return;
            }
            var assembly = ctx.Assembly;

            foreach (var table in Tables)
            {
                bool zeroAble;
                string actualTable;
                if (table.EndsWith("?"))
                {
                    zeroAble = true;
#if !LUBAN_ASSISTANT
                    actualTable = table[0..^1];
#else
                    actualTable = table.Substring(0, table.Length - 1);
#endif
                }
                else
                {
                    zeroAble = false;
                    actualTable = table;
                }
                if (zeroAble && key.Apply(IsDefaultValue.Ins))
                {
                    return;
                }
                DefTable ct = assembly.GetCfgTable(actualTable);
                var recordMap = assembly.GetTableDataInfo(ct).FinalRecordMap;
                if (/*recordMap != null &&*/ recordMap.ContainsKey(key))
                {
                    return;
                }
            }

            foreach (var table in Tables)
            {
                string actualTable;
                if (table.EndsWith("?"))
                {
#if !LUBAN_ASSISTANT
                    actualTable = table[0..^1];
#else
                    actualTable = table.Substring(0, table.Length - 1);
#endif
                }
                else
                {
                    actualTable = table;
                }
                DefTable ct = assembly.GetCfgTable(actualTable);
                string source = ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;
                assembly.Agent.Error("记录 {0} = {1} (来自文件:{2}) 在引用表:{3} 中不存在", ValidatorContext.CurrentRecordPath, key, source, table);
            }
        }

        public void Compile(DefField def)
        {
            if (Tables.Count == 0)
            {
                throw new Exception($"结构:{ def.HostType.FullName } 字段: { def.Name}  ref 不能为空");
            }

            foreach (var table in Tables)
            {
#if !LUBAN_ASSISTANT
                string actualTable = table.EndsWith("?") ? table[0..^1] : table;
#else
                string actualTable = table.EndsWith("?") ? table.Substring(0, table.Length - 1) : table;
#endif
                var ct = def.Assembly.GetCfgTable(actualTable);
                if (ct == null)
                {
                    throw new Exception($"结构:{def.HostType.FullName} 字段:{def.Name} ref:{table} 不存在");
                }
                if (ct.IsOneValueTable)
                {
                    throw new Exception($"结构:{def.HostType.FullName} 字段:{def.Name} ref:{table} 是单值表，不能执行引用检查");
                }
            }

        }
    }
}
