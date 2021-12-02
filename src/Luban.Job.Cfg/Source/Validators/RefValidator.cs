using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Validators
{
    [Validator("ref")]
    public class RefValidator : IValidator
    {

        public List<string> Tables { get; }

        public string FirstTable => GetActualTableName(Tables[0]);

        public TType Type { get; }

        public bool GenRef { get; private set; }

        public RefValidator(TType type, string tablesStr)
        {
            Type = type;
            this.Tables = DefUtil.TrimBracePairs(tablesStr).Split(',').Select(s => s.Trim()).ToList();
        }

        public void Validate(ValidatorContext ctx, TType type, DType key)
        {
            // 对于可空字段，跳过检查
            if (type.IsNullable && key == null)
            {
                return;
            }
            var assembly = ctx.Assembly;

#if !LUBAN_LITE
            foreach (var table in Tables)
            {
                var (actualTable, field, zeroAble) = ParseRefString(table);
                if (zeroAble && key.Apply(IsDefaultValue.Ins))
                {
                    return;
                }
                DefTable ct = assembly.GetCfgTable(actualTable);

                switch (ct.Mode)
                {
                    case ETableMode.ONE:
                    {
                        throw new NotSupportedException($"{actualTable} 是singleton表，不支持ref");
                    }
                    case ETableMode.MAP:
                    {
                        var recordMap = assembly.GetTableDataInfo(ct).FinalRecordMap;
                        if (/*recordMap != null &&*/ recordMap.ContainsKey(key))
                        {
                            return;
                        }
                        break;
                    }
                    case ETableMode.LIST:
                    {
                        var recordMap = assembly.GetTableDataInfo(ct).FinalRecordMapByIndexs[field];
                        if (recordMap.ContainsKey(key))
                        {
                            return;
                        }
                        break;
                    }
                    default: throw new NotSupportedException();
                }
            }

            string source = ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;
            foreach (var table in Tables)
            {
                var (actualTable, field, zeroAble) = ParseRefString(table);
                DefTable ct = assembly.GetCfgTable(actualTable);
                assembly.Agent.Error("记录 {0} = {1} (来自文件:{2}) 在引用表:{3} 中不存在", ValidatorContext.CurrentRecordPath, key, source, table);
            }
#endif
        }


        private static string GetActualTableName(string table)
        {
            var (actualTable, _, _) = ParseRefString(table);
            return actualTable;
        }

        private static (string TableName, string FieldName, bool IgnoreDefault) ParseRefString(string refStr)
        {
            bool ignoreDefault = false;

            if (refStr.EndsWith("?"))
            {
                refStr = refStr.Substring(0, refStr.Length - 1);
                ignoreDefault = true;
            }

            string tableName;
            string fieldName;
            int sepIndex = refStr.IndexOf('@');
            if (sepIndex >= 0)
            {
                tableName = refStr.Substring(sepIndex + 1);
                fieldName = refStr.Substring(0, sepIndex);
            }
            else
            {
                tableName = refStr;
                fieldName = "";
            }
            return (tableName, fieldName, ignoreDefault);
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
            bool first = true;
            foreach (var table in Tables)
            {
                var (actualTable, indexName, ignoreDefault) = ParseRefString(table);
                var ct = assembly.GetCfgTable(actualTable);
                if (ct == null)
                {
                    throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 不存在");
                }
                if (!ct.NeedExport)
                {
                    throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' ref 引用的表:'{actualTable}' 没有导出");
                }
                if (ct.IsOneValueTable)
                {
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是singleton表，索引字段不能为空");
                    }
                    else
                    {
                        if (!ct.ValueTType.Bean.TryGetField(fieldName, out var indexField, out _))
                        {
                            throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} value_type:{ct.ValueTType.Bean.FullName} 未包含索引字段:{fieldName}");
                        }
                        if (!(indexField.CType is TMap tmap))
                        {
                            throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} value_type:{ct.ValueTType.Bean.FullName} 索引字段:{fieldName} type:{indexField.CType.TypeName} 不是map类型");
                        }
                        if (tmap.KeyType.TypeName != Type.TypeName)
                        {
                            throw new Exception($"结构:{hostTypeName} 字段:{fieldName} 类型:'{Type.TypeName}' 与被引用的表:{actualTable} value_type:{ct.ValueTType.Bean.FullName} 索引字段:{fieldName} key_type:{tmap.KeyType.TypeName} 不一致");
                        }
                    }
                }
                else if (ct.IsMapTable)
                {
                    if (first && Tables.Count == 1)
                    {
                        GenRef = true;
                    }
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是map表，不能索引子字段");
                    }
                    var keyType = ct.KeyTType;
                    if (keyType.TypeName != Type.TypeName)
                    {
                        throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' 类型:'{Type.TypeName}' 与 被引用的map表:'{actualTable}' key类型:'{keyType.TypeName}' 不一致");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(indexName))
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是list表，必须显式指定索引字段");
                    }
                    var indexField = ct.IndexList.Find(k => k.IndexField.Name == indexName);
                    if (indexField.Type == null)
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} 索引字段:{indexName} 不是被引用的list表:{actualTable} 的索引字段，合法值为'{ct.Index}'之一");
                    }
                    if (indexField.Type.TypeName != Type.TypeName)
                    {
                        throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' 类型:'{Type.TypeName}' 与 被引用的list表:'{actualTable}' key:{indexName} 类型:'{indexField.Type.TypeName}' 不一致");
                    }
                }
                first = false;
            }
        }
    }
}
