// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Diagnostics;
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

    public override void Compile(DefField field, TType type)
    {
        this._tables = DefUtil.TrimBracePairs(Args).Split(',').Select(s => s.Trim()).ToList();
        if (_tables.Count == 0)
        {
            throw new LubanException("error.validator.ref.empty", field);
        }

        var assembly = field.Assembly;
        foreach (var table in _tables)
        {
            var (actualTable, indexName, ignoreDefault) = ParseRefString(table);
            DefTable ct;
            DefRefGroup refGroup;
            if ((ct = assembly.GetCfgTable(actualTable)) != null)
            {
                CompileTable(field, type, ct, indexName, ignoreDefault);
            }
            else if ((refGroup = assembly.GetRefGroup(actualTable)) != null)
            {
                if (!string.IsNullOrWhiteSpace(indexName))
                {
                    throw new LubanException("error.validator.ref.refgroup_index", actualTable, indexName);
                }
                foreach (var rawRefTableName in refGroup.Refs)
                {
                    var (actualRefTableName, refIndex, refIgnoreDefault) = ParseRefString(rawRefTableName);
                    DefTable subTable = assembly.GetCfgTable(actualRefTableName);
                    if (subTable == null)
                    {
                        throw new LubanException("error.validator.ref.refgroup_missing", field, actualTable, actualRefTableName);
                    }
                    CompileTable(field, type, subTable, refIndex, ignoreDefault || refIgnoreDefault);
                }
            }
            else
            {
                throw new LubanException("error.validator.ref.missing", field, actualTable);
            }
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType key)
    {
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
                        /*
                        if (!rec.IsNotFiltered(excludeTags))
                        {
                            s_logger.Error("记录 {} = {} (来自文件:{}) 在引用表:{} 中存在，但导出时被过滤了",
                                RecordPath, key, Source, defTable.FullName);
                        }
                        */
                        return;
                    }
                    break;
                }
                case TableMode.LIST:
                {
                    var recordMap = genCtx.GetTableDataInfo(defTable).FinalRecordMapByIndexs[field];
                    if (recordMap.TryGetValue(key, out Record rec))
                    {
                        /*
                        if (!rec.IsNotFiltered(excludeTags))
                        {
                            s_logger.Error("记录 {} = {} (来自文件:{}) 在引用表:{} 中存在，但导出时被过滤了",
                                RecordPath, key, Source, defTable.FullName);
                        }
                        */
                        return;
                    }
                    break;
                }
                default:
                    throw new NotSupportedException();
            }
        }

        foreach (var table in _compiledTables)
        {
            s_logger.Error(MessageCatalog.Format("error.validator.ref.not_found", RecordPath, key, Source, table.Table.FullName));
        }
        GenerationContext.Current.LogValidatorFail(this);
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

    private void CompileTable(DefField field, TType type, DefTable table, string indexName, bool ignoreDefault)
    {
        _compiledTables.Add((table, indexName, ignoreDefault));

        string actualTable = table.FullName;
        string fieldTypeName = type.TypeName;
        string valueTypeName = table.ValueTType.DefBean.FullName;
        if (!table.NeedExport() && field.NeedExport() && field.HostType.Assembly.ExportTables.Any(t => t.ValueTType.DefBean.IsAssignableFrom(field.HostType)))
        {
            throw new LubanException("error.validator.ref.not_exported", field, actualTable);
        }
        if (table.IsSingletonTable)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new LubanException("error.validator.ref.singleton_index", field, actualTable);
            }
            if (!table.ValueTType.DefBean.TryGetField(indexName, out var indexField, out _))
            {
                throw new LubanException("error.validator.ref.index_missing", field, actualTable, valueTypeName, indexName);
            }
            if (!(indexField.CType is TMap tmap))
            {
                throw new LubanException("error.validator.ref.index_not_map", field, actualTable, valueTypeName, indexName, indexField.CType.TypeName);
            }
            if (tmap.KeyType.TypeName != fieldTypeName)
            {
                throw new LubanException("error.validator.ref.type_mismatch_map_index", field, type.TypeName, actualTable, valueTypeName, indexName, tmap.KeyType.TypeName);
            }

        }
        else if (table.IsMapTable)
        {
            if (!string.IsNullOrEmpty(indexName))
            {
                throw new LubanException("error.validator.ref.map_no_sub_index", field, actualTable);
            }
            var keyType = table.KeyTType;
            if (keyType.TypeName != fieldTypeName)
            {
                throw new LubanException("error.validator.ref.type_mismatch_map_key", field, fieldTypeName, actualTable, keyType.TypeName);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new LubanException("error.validator.ref.list_need_index", field, actualTable);
            }
            var indexField = table.IndexList.Find(k => k.IndexField.Name == indexName);
            if (indexField?.Type == null)
            {
                throw new LubanException("error.validator.ref.list_invalid_index", field, indexName, actualTable, table.Index);
            }
            if (indexField.Type.TypeName != fieldTypeName)
            {
                throw new LubanException("error.validator.ref.type_mismatch_list_key", field, fieldTypeName, actualTable, indexName, indexField.Type.TypeName);
            }
        }
    }
}
