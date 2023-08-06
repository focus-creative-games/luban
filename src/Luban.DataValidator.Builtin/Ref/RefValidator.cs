using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Ref;

[Validator("ref")]
public class RefValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private List<string> _tables;

    private readonly List<(DefTable Table, string Index, bool IgnoreDefault)> _compiledTables = new();

    public RefValidator()
    {

    }

    public override void Compile(DefField field)
    {
        this._tables = DefUtil.TrimBracePairs(Args).Split(',').Select(s => s.Trim()).ToList();
        string hostTypeName = field.HostType.FullName;
        string fieldName = field.Name;
        if (_tables.Count == 0)
        {
            throw new Exception($"结构:'{hostTypeName}' 字段: '{fieldName}' ref 不能为空");
        }

        var assembly = field.Assembly;
        foreach (var table in _tables)
        {
            var (actualTable, indexName, ignoreDefault) = ParseRefString(table);
            DefTable ct;
            DefRefGroup refGroup;
            if ((ct = assembly.GetCfgTable(actualTable)) != null)
            {
                CompileTable(field, ct, indexName, ignoreDefault);
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
                    CompileTable(field, subTable, refIndex, ignoreDefault || refIgnoreDefault);
                }
            }
            else
            {
                throw new Exception($"结构:'{hostTypeName}' 字段:'{fieldName}' ref:'{actualTable}' 不存在");
            }
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType key)
    {
        // 对于可空字段，跳过检查
        if (type.IsNullable && key == null)
        {
            return;
        }
        var genCtx = GenerationContext.Current;
        var excludeTags = genCtx.ExcludeTags;

        foreach (var tableInfo in _compiledTables)
        {
            var (defTable, field, zeroAble) = tableInfo;
            if (zeroAble && key.Apply(IsDefaultValueVisitor.Ins))
            {
                return;
            }

            switch (defTable.Mode)
            {
                case TableMode.ONE:
                {
                    throw new NotSupportedException($"{defTable.FullName} 是singleton表，不支持ref");
                }
                case TableMode.MAP:
                {
                    var recordMap = genCtx.GetTableDataInfo(defTable).FinalRecordMap;
                    if (recordMap.TryGetValue(key, out Record rec))
                    {
                        if (!rec.IsNotFiltered(excludeTags))
                        {
                            s_logger.Error("记录 {} = {} (来自文件:{}) 在引用表:{} 中存在，但导出时被过滤了",
                                RecordPath, key, Source, defTable.FullName);
                        }
                        return;
                    }
                    break;
                }
                case TableMode.LIST:
                {
                    var recordMap = genCtx.GetTableDataInfo(defTable).FinalRecordMapByIndexs[field];
                    if (recordMap.TryGetValue(key, out Record rec))
                    {
                        if (!rec.IsNotFiltered(excludeTags))
                        {
                            s_logger.Error("记录 {} = {} (来自文件:{}) 在引用表:{} 中存在，但导出时被过滤了",
                                RecordPath, key, Source, defTable.FullName);
                        }
                        return;
                    }
                    break;
                }
                default: throw new NotSupportedException();
            }
        }

        foreach (var table in _compiledTables)
        {
            s_logger.Error("记录 {} = {} (来自文件:{}) 在引用表:{} 中不存在", RecordPath, key, Source, table.Table.FullName);
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

    private void CompileTable(DefField field, DefTable table, string indexName, bool ignoreDefault)
    {
        _compiledTables.Add((table, indexName, ignoreDefault));

        string actualTable = table.FullName;
        string hostTypeName = field.HostType.FullName;
        string fieldName = field.Name;
        string fieldTypeName = field.CType.TypeName;
        string valueTypeName = table.ValueTType.DefBean.FullName;
        if (!table.NeedExport())
        {
            throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' ref 引用的表:'{actualTable}' 没有导出");
        }
        if (table.IsSingletonTable)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是singleton表，索引字段不能为空");
            }
            if (!table.ValueTType.DefBean.TryGetField(indexName, out var indexField, out _))
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} value_type:{valueTypeName} 未包含索引字段:{indexName}");
            }
            if (!(indexField.CType is TMap tmap))
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} value_type:{valueTypeName} 索引字段:{indexName} type:{indexField.CType.TypeName} 不是map类型");
            }
            if (tmap.KeyType.TypeName != fieldTypeName)
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} 类型:'{field.CType.TypeName}' 与被引用的表:{actualTable} value_type:{valueTypeName} 索引字段:{indexName} key_type:{tmap.KeyType.TypeName} 不一致");
            }
            
        }
        else if (table.IsMapTable)
        {
            if (!string.IsNullOrEmpty(indexName))
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是map表，不能索引子字段");
            }
            var keyType = table.KeyTType;
            if (keyType.TypeName != fieldTypeName)
            {
                throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' 类型:'{fieldTypeName}' 与 被引用的map表:'{actualTable}' key类型:'{keyType.TypeName}' 不一致");
            }
        }
        else
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} ref:{actualTable} 是list表，必须显式指定索引字段");
            }
            var indexField = table.IndexList.Find(k => k.IndexField.Name == indexName);
            if (indexField.Type == null)
            {
                throw new Exception($"结构:{hostTypeName} 字段:{fieldName} 索引字段:{indexName} 不是被引用的list表:{actualTable} 的索引字段，合法值为'{table.Index}'之一");
            }
            if (indexField.Type.TypeName != fieldTypeName)
            {
                throw new Exception($"type:'{hostTypeName}' field:'{fieldName}' 类型:'{fieldTypeName}' 与 被引用的list表:'{actualTable}' key:{indexName} 类型:'{indexField.Type.TypeName}' 不一致");
            }
        }
    }
}