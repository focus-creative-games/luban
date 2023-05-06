using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
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

        private readonly List<(DefTable Table, string Index, bool IgnoreDefault)> _compiledTables = new();

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

            foreach (var tableInfo in _compiledTables)
            {
                var (defTable, field, zeroAble) = tableInfo;
                if (zeroAble && key.Apply(IsDefaultValue.Ins))
                {
                    return;
                }

                switch (defTable.Mode)
                {
                    case ETableMode.ONE:
                    {
                        throw new NotSupportedException($"{defTable.FullName} 是singleton表，不支持ref");
                    }
                    case ETableMode.MAP:
                    {
                        var recordMap = assembly.GetTableDataInfo(defTable).FinalRecordMap;
                        if (recordMap.TryGetValue(key, out Record rec))
                        {
                            if (!rec.IsNotFiltered(assembly.ExcludeTags))
                            {
                                string locationFile = ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;
                                assembly.Agent.Error("记录 {0} = {1} (来自文件:{2}) 在引用表:{3} 中存在，但导出时被过滤了",
                                    ValidatorContext.CurrentRecordPath, key, locationFile, defTable.FullName);
                            }
                            return;
                        }
                        break;
                    }
                    case ETableMode.LIST:
                    {
                        var recordMap = assembly.GetTableDataInfo(defTable).FinalRecordMapByIndexs[field];
                        if (recordMap.TryGetValue(key, out Record rec))
                        {
                            if (!rec.IsNotFiltered(assembly.ExcludeTags))
                            {
                                string locationFile = ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;
                                assembly.Agent.Error("记录 {0} = {1} (来自文件:{2}) 在引用表:{3} 中存在，但导出时被过滤了",
                                    ValidatorContext.CurrentRecordPath, key, locationFile, defTable.FullName);
                            }
                            return;
                        }
                        break;
                    }
                    default: throw new NotSupportedException();
                }
            }

            string source = ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;
            foreach (var table in _compiledTables)
            {
                assembly.Agent.Error("记录 {0} = {1} (来自文件:{2}) 在引用表:{3} 中不存在", ValidatorContext.CurrentRecordPath, key, source, table.Table.FullName);
            }
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
                throw new Exception($"结构:'{hostTypeName}' 字段: '{fieldName}' ref 不能为空");
            }

            var assembly = ((DefField)def).Assembly;
            bool anyRefGroup = false;
            foreach (var table in Tables)
            {
                var (actualTable, indexName, ignoreDefault) = ParseRefString(table);
                DefTable ct;
                DefRefGroup refGroup;
                if ((ct = assembly.GetCfgTable(actualTable)) != null)
                {
                    CompileTable(def, ct, indexName, ignoreDefault);
                }
                else if ((refGroup = assembly.GetRefGroup(actualTable)) != null)
                {
                    if (!string.IsNullOrWhiteSpace(indexName))
                    {
                        throw new Exception($"refgroup:'{actualTable}' index:'{indexName}' 必须为空");
                    }
                    foreach (var rawRefTableName in refGroup.Refs)
                    {
                        var (actualRefTableName, refIndex, refIgnoreDefault) = ParseRefString(rawRefTableName);
                        DefTable subTable = assembly.GetCfgTable(actualRefTableName);
                        if (subTable == null)
                        {
                            throw new Exception($"结构:'{hostTypeName}' 字段:'{fieldName}' refgroup:'{actualTable}' ref:'{actualRefTableName}' 不存在");
                        }
                        CompileTable(def, subTable, refIndex, ignoreDefault || refIgnoreDefault);
                    }
                    anyRefGroup = true;
                }
                else
                {
                    throw new Exception($"结构:'{hostTypeName}' 字段:'{fieldName}' ref:'{actualTable}' 不存在");
                }
            }
            if (!anyRefGroup && _compiledTables.Count == 1 && (_compiledTables[0].Table is DefTable t && t.IsMapTable && t.NeedExport))
            {
                // 只引用一个表时才生成ref代码。
                // 如果被引用的表没有导出，生成ref没有意义，还会产生编译错误
                GenRef = true;
            }
        }

        private void CompileTable(DefFieldBase def, DefTable ct, string indexName, bool ignoreDefault)
        {
            _compiledTables.Add((ct, indexName, ignoreDefault));

            string actualTable = ct.FullName;
            string hostTypeName = def.HostType.FullName;
            string fieldName = def.Name;
            //if (!ct.NeedExport)
            //{
            //    throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' ref 引用的表:'{actualTable}' 没有导出");
            //}
            if (ct.IsOneValueTable)
            {
                if (string.IsNullOrEmpty(indexName))
                {
                    throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是singleton表，索引字段不能为空");
                }
                else
                {
                    if (!ct.ValueTType.Bean.TryGetField(indexName, out var indexField, out _))
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} value_type:{ct.ValueTType.Bean.FullName} 未包含索引字段:{indexName}");
                    }
                    if (!(indexField.CType is TMap tmap))
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} value_type:{ct.ValueTType.Bean.FullName} 索引字段:{indexName} type:{indexField.CType.TypeName} 不是map类型");
                    }
                    if (tmap.KeyType.TypeName != Type.TypeName)
                    {
                        throw new Exception($"结构:{hostTypeName} 字段:{fieldName} 类型:'{Type.TypeName}' 与被引用的表:{actualTable} value_type:{ct.ValueTType.Bean.FullName} 索引字段:{indexName} key_type:{tmap.KeyType.TypeName} 不一致");
                    }
                }
            }
            else if (ct.IsMapTable)
            {
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
        }
    }
}
