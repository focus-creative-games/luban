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

﻿using Luban.Datas;
using Luban.Utils;

namespace Luban.Defs;

public class TableDataInfo
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public DefTable Table { get; }

    public List<Record> MainRecords { get; }

    public List<Record> PatchRecords { get; }

    public List<Record> FinalRecords { get; private set; }

    public Dictionary<DType, Record> FinalRecordMap { get; private set; }

    public Dictionary<string, Dictionary<DType, Record>> FinalRecordMapByIndexs { get; private set; }

    public TableDataInfo(DefTable table, List<Record> mainRecords, List<Record> patchRecords)
    {
        Table = table;
        MainRecords = mainRecords;
        PatchRecords = patchRecords;

        BuildIndexs();

        int index = 0;
        foreach (var record in FinalRecords)
        {
            record.AutoIndex = index++;
        }

        if (table.IsSingletonTable && FinalRecords.Count != 1)
        {
            throw new Exception($"配置表 {table.FullName} 是单值表 mode=one,但数据个数:{FinalRecords.Count} != 1");
        }
    }

    private void BuildIndexs()
    {
        List<Record> mainRecords = MainRecords;
        List<Record> patchRecords = PatchRecords;

        // 这么大费周张是为了保证被覆盖的id仍然保持原来的顺序，而不是出现在最后
        int index = 0;
        var recordIndex = new Dictionary<Record, int>();
        var overrideRecords = new HashSet<Record>();
        foreach (var r in mainRecords)
        {
            if (recordIndex.TryAdd(r, index))
            {
                index++;
            }
        }
        if (patchRecords != null)
        {
            foreach (var r in patchRecords)
            {
                if (recordIndex.TryAdd(r, index))
                {
                    index++;
                }
            }
        }

        var table = Table;
        // TODO 有一个微妙的问题，ref检查虽然通过，但ref的记录有可能未导出
        switch (Table.Mode)
        {
            case TableMode.ONE:
            {
                // TODO 如果此单例表使用tag,有多个记录，则patchRecords会覆盖全部。
                // 好像也挺有道理的，毕竟没有key，无法区分覆盖哪个
                if (patchRecords != null && patchRecords.Count > 0)
                {
                    mainRecords = patchRecords;
                }
                FinalRecords = mainRecords;
                break;
            }
            case TableMode.MAP:
            {
                var recordMap = new Dictionary<DType, Record>();
                foreach (Record r in mainRecords)
                {
                    DType key = r.Data.Fields[table.IndexFieldIdIndex];
                    if (!recordMap.TryAdd(key, r))
                    {
                        throw new Exception($@"配置表 '{table.FullName}' 主文件 主键字段:'{table.Index}' 主键值:'{key}' 重复.
        记录1 来自文件:{r.Source}
        记录2 来自文件:{recordMap[key].Source}
");
                    }
                }
                if (patchRecords != null && patchRecords.Count > 0)
                {
                    foreach (Record r in patchRecords)
                    {
                        DType key = r.Data.Fields[table.IndexFieldIdIndex];
                        if (recordMap.TryGetValue(key, out var old))
                        {
                            if (overrideRecords.Contains(old))
                            {
                                throw new Exception($"配置表 '{table.FullName}' 主文件 主键字段:'{table.Index}' 主键值:'{key}' 被patch多次覆盖，请检查patch是否有重复记录");
                            }
                            s_logger.Debug("配置表 {} 分支文件 主键:{} 覆盖 主文件记录", table.FullName, key);
                            mainRecords[recordIndex[old]] = r;
                        }
                        else
                        {
                            mainRecords.Add(r);
                        }
                        overrideRecords.Add(r);
                        recordMap[key] = r;
                    }
                }
                FinalRecords = mainRecords;
                FinalRecordMap = recordMap;
                break;
            }
            case TableMode.LIST:
            {
                if (patchRecords != null && patchRecords.Count > 0)
                {
                    throw new Exception($"配置表 '{table.FullName}' 是list表.不支持patch");
                }
                var recordMapByIndexs = new Dictionary<string, Dictionary<DType, Record>>();
                if (table.IsUnionIndex)
                {
                    var unionRecordMap = new Dictionary<List<DType>, Record>(ListEqualityComparer<DType>.Default); // comparetor
                    foreach (Record r in mainRecords)
                    {
                        var unionKeys = table.IndexList.Select(idx => r.Data.Fields[idx.IndexFieldIdIndex]).ToList();
                        if (!unionRecordMap.TryAdd(unionKeys, r))
                        {
                            throw new Exception($@"配置表 '{table.FullName}' 主文件 主键字段:'{table.Index}' 主键值:'{StringUtil.CollectionToString(unionKeys)}' 重复.
        记录1 来自文件:{r.Source}
        记录2 来自文件:{unionRecordMap[unionKeys].Source}
");
                        }
                    }

                    // 联合索引的 独立子索引允许有重复key
                    foreach (var indexInfo in table.IndexList)
                    {
                        var recordMap = new Dictionary<DType, Record>();
                        foreach (Record r in mainRecords)
                        {
                            DType key = r.Data.Fields[indexInfo.IndexFieldIdIndex];
                            recordMap[key] = r;
                        }
                        recordMapByIndexs.Add(indexInfo.IndexField.Name, recordMap);
                    }
                }
                else
                {
                    foreach (var indexInfo in table.IndexList)
                    {
                        var recordMap = new Dictionary<DType, Record>();
                        foreach (Record r in mainRecords)
                        {
                            DType key = r.Data.Fields[indexInfo.IndexFieldIdIndex];
                            if (!recordMap.TryAdd(key, r))
                            {
                                throw new Exception($@"配置表 '{table.FullName}' 主文件 主键字段:'{indexInfo.IndexField.Name}' 主键值:'{key}' 重复.
        记录1 来自文件:{r.Source}
        记录2 来自文件:{recordMap[key].Source}
");
                            }
                        }
                        recordMapByIndexs.Add(indexInfo.IndexField.Name, recordMap);
                    }
                }
                this.FinalRecordMapByIndexs = recordMapByIndexs;
                FinalRecords = mainRecords;
                break;
            }
            default:
                throw new Exception($"unknown mode:{Table.Mode}");
        }
    }
}
