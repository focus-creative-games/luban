using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Validators
{
    [Validator("ref")]
    public class RefValidator : IValidator
    {

        public static string GetActualTableName(string table)
        {
#if !LUBAN_LITE
            return table.EndsWith("?") ? table[0..^1] : table;
#else
            return table.EndsWith("?") ? table.Substring(0, table.Length - 1) : table;
#endif
        }

        public List<string> Tables { get; }

        public string FirstTable => GetActualTableName(Tables[0]);

        public TType Type { get; }

        public RefValidator(TType type, string tablesStr)
        {
            Type = type;
            this.Tables = new List<string>(tablesStr.Split(','));
        }

        public void Validate(ValidatorContext ctx, TType type, DType key)
        {
            // 对于可空字段，跳过检查
            if (type.IsNullable && key == null)
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
#if !LUBAN_LITE
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
#if !LUBAN_LITE
                var recordMap = assembly.GetTableDataInfo(ct).FinalRecordMap;
                if (/*recordMap != null &&*/ recordMap.ContainsKey(key))
                {
                    return;
                }
#endif
            }

            foreach (var table in Tables)
            {
                string actualTable;
                if (table.EndsWith("?"))
                {
#if !LUBAN_LITE
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

        public void Compile(DefFieldBase def)
        {
            string hostTypeName = def.HostType.FullName;
            string fieldName = def.Name;
            if (Tables.Count == 0)
            {
                throw new Exception($"结构:{ hostTypeName } 字段: { fieldName}  ref 不能为空");
            }

            var assembly = ((DefField)def).Assembly;
            foreach (var table in Tables)
            {
#if !LUBAN_LITE
                string actualTable = table.EndsWith("?") ? table[0..^1] : table;
#else
                string actualTable = table.EndsWith("?") ? table.Substring(0, table.Length - 1) : table;
#endif
                var ct = assembly.GetCfgTable(actualTable);
                if (ct == null)
                {
                    throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{table} 不存在");
                }
                if (!ct.NeedExport)
                {
                    throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' ref 引用的表:'{table}' 没有导出");
                }
                if (ct.IsOneValueTable)
                {
                    throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{table} 是单值表，不能执行引用检查");
                }
                var keyType = ct.KeyTType;
                if (keyType.GetType() != Type.GetType())
                {
                    throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' 类型:'{Type.GetType()}' 与 被引用的表:'{ct.FullName}' key类型:'{keyType.GetType()}' 不一致");
                }
            }
        }
    }
}
