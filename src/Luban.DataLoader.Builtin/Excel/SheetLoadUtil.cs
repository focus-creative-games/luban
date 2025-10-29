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

﻿using ExcelDataReader;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.Excel;

public static class SheetLoadUtil
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static System.Text.Encoding DetectCsvEncoding(Stream fs)
    {
        Ude.CharsetDetector cdet = new Ude.CharsetDetector();
        cdet.Feed(fs);
        cdet.DataEnd();
        fs.Seek(0, SeekOrigin.Begin);
        if (cdet.Charset != null)
        {
            s_logger.Debug("Charset: {}, confidence: {}", cdet.Charset, cdet.Confidence);
            return System.Text.Encoding.GetEncoding(cdet.Charset) ?? System.Text.Encoding.Default;
        }
        else
        {
            return System.Text.Encoding.Default;
        }
    }

    private static readonly AsyncLocal<string> s_curExcel = new();

    public static IEnumerable<RawSheet> LoadRawSheets(string rawUrl, string sheetName, Stream stream)
    {
        s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
        s_curExcel.Value = rawUrl;
        string ext = Path.GetExtension(rawUrl);
        using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(stream) : ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration() { FallbackEncoding = DetectCsvEncoding(stream) }))
        {
            do
            {
                if (sheetName == null || reader.Name == sheetName)
                {
                    RawSheet sheet;
                    try
                    {
                        sheet = ParseRawSheet(reader);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"excel:{rawUrl} sheet:{reader.Name} 读取失败.", e);
                    }
                    if (sheet != null)
                    {
                        yield return sheet;
                    }
                }
            } while (reader.NextResult());
        }
    }

    private static RawSheet ParseRawSheet(IExcelDataReader reader)
    {
        bool orientRow;

        if (!TryParseMeta(reader, out orientRow))
        {
            return null;
        }
        var cells = ParseRawSheetContent(reader, orientRow, false);
        ValidateTitles(cells);
        var title = ParseTitle(cells, reader.MergeCells, orientRow);
        cells.RemoveAll(c => IsNotDataRow(c));
        return new RawSheet() { Title = title, SheetName = reader.Name, Cells = cells };
    }


    private static readonly HashSet<string> s_knownSpecialTags = new()
    {
        "var",
        "+",
        "type",
        "desc",
        "comment",
        "column",
        "group",
    };

    private const char s_sep = '#';

    private static void ValidateTitles(List<List<Cell>> rows)
    {
        foreach (var row in rows)
        {
            if (row.Count == 0)
            {
                continue;
            }
            string rowTag = row[0].Value?.ToString()?.ToLower()?.Trim();
            if (string.IsNullOrEmpty(rowTag))
            {
                continue;
            }
            if (!rowTag.StartsWith("##"))
            {
                break;
            }
            var tags = StringUtil.SplitStringWithEscape(rowTag.Substring(2), s_sep).Where(s => !string.IsNullOrEmpty(s)).ToList();
            foreach (string tag in tags)
            {
                if (!s_knownSpecialTags.Contains(tag))
                {
                    s_logger.Error("文件:'{}' 行标签:'{}' 包含未知tag:'{}'，是否有拼写错误?", s_curExcel.Value, rowTag, tag);
                }
            }
        }
    }

    private static bool IsNotDataRow(List<Cell> row)
    {
        if (row.Count == 0)
        {
            return true;
        }
        var s = row[0].Value?.ToString()?.Trim();
        return !string.IsNullOrEmpty(s) && s.StartsWith("##");
    }

    public static Title ParseTitle(List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow)
    {
        var rootTitle = new Title()
        {
            Root = true,
            Name = "__root__",
            Tags = new Dictionary<string, string>(),
            FromIndex = 0,
            ToIndex = cells.Select(r => r.Count).Max() - 1
        };

        if (!TryFindTopTitle(cells, out var topTitleRowIndex))
        {
            throw new Exception($"没有定义任何有效 标题行");
        }
        //titleRowNum = GetTitleRowNum(mergeCells, orientRow);

        ParseSubTitles(rootTitle, cells, mergeCells, orientRow, topTitleRowIndex + 1);

        rootTitle.Init();

        if (rootTitle.SubTitleList.Count == 0)
        {
            throw new Exception($"没有定义任何有效 列");
        }
        return rootTitle;
    }

    private static bool TryFindTopTitle(List<List<Cell>> cells, out int rowIndex)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            var row = cells[i];
            if (row.Count == 0)
            {
                break;
            }
            string rowTag = row[0].Value?.ToString()?.ToLower()?.Trim() ?? "";
            if (!rowTag.StartsWith("##"))
            {
                break;
            }
            if (rowTag.Substring(2).IndexOf('&') >= 0)
            {
                throw new Exception($"excel标题头不再使用'&'作为分割符，请改为'{s_sep}'");
            }
            var tags = StringUtil.SplitStringWithEscape(rowTag.Substring(2), s_sep).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (tags.Contains("field") || tags.Contains("var") || tags.Contains("+"))
            {
                rowIndex = i;
                return true;
            }
            // 出于历史兼容性，对第一行特殊处理，如果不包含任何tag或者只包含column，则也认为是标题行
            if (i == 0 && (tags.Count == 0 || (tags.Count == 1 && tags[0] == "column")))
            {
                rowIndex = i;
                return true;
            }
        }
        rowIndex = 0;
        return false;
    }

    private static bool TryFindNextSubFieldRowIndex(List<List<Cell>> cells, int startRowIndex, out int rowIndex)
    {
        for (int i = startRowIndex; i < cells.Count; i++)
        {
            var row = cells[i];
            if (row.Count == 0)
            {
                break;
            }
            string rowTag = row[0].Value?.ToString()?.ToLower()?.Trim() ?? "";
            if (rowTag == "##field" || rowTag == "##var" || rowTag == "##+")
            {
                rowIndex = i;
                return true;
            }
            else if (!rowTag.StartsWith("##"))
            {
                break;
            }
        }
        rowIndex = 0;
        return false;
    }

    private static bool IsIgnoreTitle(string title)
    {
        return string.IsNullOrEmpty(title) || title.StartsWith('#');
    }

    public static (string Name, Dictionary<string, string> Tags) ParseNameAndMetaAttrs(string nameAndAttrs)
    {
        if (nameAndAttrs.Contains('&'))
        {
            throw new Exception($"excel标题头不再使用'&'作为分割符，请改为'{s_sep}'");
        }
        var attrs = StringUtil.SplitStringWithEscape(nameAndAttrs, s_sep);

        string titleName = attrs[0];
        var tags = new Dictionary<string, string>();
        // *  开头的表示是多行
        if (titleName.StartsWith('*'))
        {
            titleName = titleName.Substring(1);
            tags.Add("multi_rows", "1");
        }
        //if (titleName.EndsWith("*"))
        //{
        //    titleName = titleName.Substring(0, titleName.Length - 1);
        //    tags.Add("multi_rows", "1");
        //}
        if (titleName.StartsWith('!'))
        {
            titleName = titleName.Substring(1);
            tags.Add("non_empty", "1");
        }
        //if (titleName.EndsWith("!"))
        //{
        //    titleName = titleName.Substring(0, titleName.Length - 1);
        //    tags.Add("non_empty", "1");
        //}
        foreach (var attrPair in attrs.Skip(1))
        {
            var pairs = attrPair.Split('=');
            if (pairs.Length != 2)
            {
                throw new Exception($"invalid title: {nameAndAttrs}");
            }
            tags.Add(pairs[0].Trim(), pairs[1].Trim());
        }
        return (titleName, tags);
    }

    private static void ParseSubTitles(Title title, List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow, int excelRowIndex)
    {
        var rowIndex = excelRowIndex - 1;
        var titleRow = cells[rowIndex];
        if (mergeCells != null)
        {
            foreach (var mergeCell in mergeCells)
            {
                Title subTitle = null;
                if (orientRow)
                {
                    //if (mergeCell.FromRow <= 1 && mergeCell.ToRow >= 1)
                    if (mergeCell.FromRow == rowIndex && mergeCell.FromColumn >= title.FromIndex && mergeCell.FromColumn <= title.ToIndex)
                    {
                        var nameAndAttrs = titleRow[mergeCell.FromColumn].Value?.ToString()?.Trim();
                        if (IsIgnoreTitle(nameAndAttrs))
                        {
                            continue;
                        }
                        var (titleName, tags) = ParseNameAndMetaAttrs(nameAndAttrs);
                        subTitle = new Title() { Name = titleName, Tags = tags, FromIndex = mergeCell.FromColumn, ToIndex = mergeCell.ToColumn };
                        //s_logger.Info("=== sheet:{sheet} title:{title}", Name, newTitle);
                    }
                }
                else
                {
                    if (mergeCell.FromColumn == rowIndex && mergeCell.FromRow >= title.FromIndex && mergeCell.FromRow <= title.ToIndex)
                    {
                        // 标题 行
                        var nameAndAttrs = titleRow[mergeCell.FromRow].Value?.ToString()?.Trim();
                        if (IsIgnoreTitle(nameAndAttrs))
                        {
                            continue;
                        }
                        var (titleName, tags) = ParseNameAndMetaAttrs(nameAndAttrs);
                        subTitle = new Title() { Name = titleName, Tags = tags, FromIndex = mergeCell.FromRow, ToIndex = mergeCell.ToRow };
                    }
                }
                if (subTitle == null)
                {
                    continue;
                }

                if (excelRowIndex < cells.Count && TryFindNextSubFieldRowIndex(cells, excelRowIndex, out int nextRowIndex))
                {
                    ParseSubTitles(subTitle, cells, mergeCells, orientRow, nextRowIndex + 1);
                }
                title.AddSubTitle(subTitle);

            }
        }

        for (int i = title.FromIndex; i <= title.ToIndex; i++)
        {
            string nameAndAttrs = i < titleRow.Count ? titleRow[i].Value?.ToString()?.Trim() : null;
            if (IsIgnoreTitle(nameAndAttrs))
            {
                continue;
            }
            var (titleName, tags) = ParseNameAndMetaAttrs(nameAndAttrs);

            Title subTitle;
            // [field,,,, field] 形成多列字段


            if (titleName.StartsWith('['))
            {
                int startIndex = i;
                titleName = titleName.Substring(1);
                bool findEndPair = false;
                for (++i; i <= title.ToIndex; i++)
                {
                    if (i >= titleRow.Count)
                    {
                        break;
                    }
                    var endNamePair = titleRow[i].Value?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(endNamePair))
                    {
                        continue;
                    }
                    if (!endNamePair.EndsWith(']') || endNamePair[0..^1] != titleName)
                    {
                        throw new Exception($"列:'[{titleName}' 后第一个有效列必须为匹配 '{titleName}]'，却发现:'{endNamePair}'");
                    }
                    findEndPair = true;
                    break;
                }
                if (!findEndPair)
                {
                    throw new Exception($"列:'[{titleName}' 未找到结束匹配列 '{titleName}]'");
                }
                subTitle = new Title() { Name = titleName, Tags = tags, FromIndex = startIndex, ToIndex = i };
            }
            else
            {
                if (title.SubTitles.TryGetValue(titleName, out subTitle))
                {
                    if (subTitle.FromIndex != i)
                    {
                        throw new Exception($"列:{titleName} 重复");
                    }
                    else
                    {
                        continue;
                    }
                }
                subTitle = new Title() { Name = titleName, Tags = tags, FromIndex = i, ToIndex = i };
            }
            if (excelRowIndex < cells.Count && TryFindNextSubFieldRowIndex(cells, excelRowIndex, out int nextRowIndex))
            {
                ParseSubTitles(subTitle, cells, mergeCells, orientRow, nextRowIndex + 1);
            }
            title.AddSubTitle(subTitle);
        }
    }

    public static bool TryParseMeta(string metaStr, out bool orientRow)
    {
        orientRow = true;

        // meta 行 必须以 ##为第一个单元格内容,紧接着 key:value 形式 表达meta属性
        if (string.IsNullOrEmpty(metaStr) || !metaStr.StartsWith("##"))
        {
            return false;
        }
        if (metaStr.Substring(2).Contains('&'))
        {
            throw new Exception($"excel标题头不再使用'&'作为分割符，请改为'{s_sep}'");
        }
        foreach (var attr in StringUtil.SplitStringWithEscape(metaStr.Substring(2), s_sep))
        {
            if (string.IsNullOrWhiteSpace(attr))
            {
                continue;
            }

            var sepIndex = attr.IndexOf('=');
            string key = sepIndex >= 0 ? attr.Substring(0, sepIndex) : attr;
            string value = sepIndex >= 0 ? attr.Substring(sepIndex + 1) : "";
            switch (key)
            {
                case "+":
                case "var":
                case "comment":
                case "type":
                {
                    break;
                }
                case "column":
                case "vertical":
                {
                    orientRow = false;
                    break;
                }
                default:
                {
                    throw new Exception($"非法单元薄 meta 属性定义 {attr}, 合法属性有: +,var,row,column,table=<tableName>");
                }
            }
        }
        return true;
    }

    public static bool TryParseMeta(IExcelDataReader reader, out bool orientRow)
    {
        if (!reader.Read() || reader.FieldCount == 0)
        {
            orientRow = true;
            return false;
        }
        string metaStr = reader.GetValue(0)?.ToString().Trim();
        return TryParseMeta(metaStr, out orientRow);
    }

    private static bool IsTypeRow(List<Cell> row)
    {
        return IsRowTagEqual(row, "##type");
    }
    private static bool IsGroupRow(List<Cell> row)
    {
        return IsRowTagEqual(row, "##group");
    }

    private static bool IsHeaderRow(List<Cell> row)
    {
        if (row.Count == 0)
        {
            return false;
        }
        var s = row[0].Value?.ToString()?.Trim();
        return !string.IsNullOrEmpty(s) && s.StartsWith("##");
    }

    private static bool IsRowTagEqual(List<Cell> row, string tag)
    {
        if (row.Count == 0)
        {
            return false;
        }
        var s = row[0].Value?.ToString()?.Trim()?.ToLower();
        return s == tag;
    }

    private static bool IsEmptyRow(List<Cell> row)
    {
        return row.All(c => string.IsNullOrWhiteSpace(c.Value?.ToString()));
    }

    const int maxEmptyRowCount = 300;

    private static List<List<Cell>> ParseRawSheetContent(IExcelDataReader reader, bool orientRow, bool headerOnly)
    {
        // TODO 优化性能
        // 几个思路
        // 1. 没有 title 的列不加载
        // 2. 空行优先跳过
        // 3. 跳过null或者empty的单元格
        var originRows = new List<List<Cell>>();
        int rowIndex = 0;
        int emptyRowCount = 0;
        do
        {
            var row = new List<Cell>();
            for (int i = 0, n = reader.FieldCount; i < n; i++)
            {
                row.Add(new Cell(rowIndex, i, reader.GetValue(i)));
            }
            ++rowIndex;
            if (IsEmptyRow(row))
            {
                ++emptyRowCount;
                if (emptyRowCount == maxEmptyRowCount)
                {
                    s_logger.Warn("excel:{filename} sheet:{sheet} 连续空行超过{}行，删除这些空行可以提升导出性能", s_curExcel.Value, reader.Name, maxEmptyRowCount);
                }
            }
            else
            {
                emptyRowCount = 0;
            }
            originRows.Add(row);
            if (orientRow && headerOnly && !IsHeaderRow(row))
            {
                break;
            }
        } while (reader.Read());

        List<List<Cell>> finalRows;

        if (orientRow)
        {
            finalRows = originRows;
        }
        else
        {
            // 转置这个行列
            int maxColumn = originRows.Select(r => r.Count).Max();
            finalRows = new List<List<Cell>>();
            for (int i = 0; i < maxColumn; i++)
            {
                var row = new List<Cell>();
                for (int j = 0; j < originRows.Count; j++)
                {
                    row.Add(i < originRows[j].Count ? originRows[j][i] : new Cell(j + 1, i, null));
                }
                finalRows.Add(row);
            }
        }
        return finalRows;
    }

    public static RawSheetTableDefInfo LoadSheetTableDefInfo(string rawUrl, string sheetName, Stream stream)
    {
        s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
        string ext = Path.GetExtension(rawUrl);
        using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(stream) : ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration() { FallbackEncoding = DetectCsvEncoding(stream) }))
        {
            do
            {
                if (sheetName == null || reader.Name == sheetName)
                {
                    try
                    {
                        var tableDefInfo = ParseSheetTableDefInfo(rawUrl, reader);
                        if (tableDefInfo != null)
                        {
                            return tableDefInfo;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"excel:{rawUrl} sheet:{reader.Name} 读取失败.", e);
                    }

                }
            } while (reader.NextResult());
        }
        throw new Exception($"{rawUrl} 没有找到有效的表定义");
    }

    private static RawSheetTableDefInfo ParseSheetTableDefInfo(string rawUrl, IExcelDataReader reader)
    {
        bool orientRow;

        if (!TryParseMeta(reader, out orientRow))
        {
            return null;
        }
        var cells = ParseRawSheetContent(reader, orientRow, true);
        var title = ParseTitle(cells, reader.MergeCells, orientRow);

        int typeRowIndex = cells.FindIndex(IsTypeRow);

        if (typeRowIndex < 0)
        {
            throw new Exception($"缺失type行。请用'##type'标识type行");
        }
        List<Cell> typeRow = cells[typeRowIndex];

        // 先找 ##desc 行，再找##comment行，最后找 ##type的下一行
        List<Cell> descRow = cells.Find(row => IsRowTagEqual(row, "##desc"));
        if (descRow == null)
        {
            descRow = cells.Find(row => IsRowTagEqual(row, "##comment"));
        }
        if (descRow == null)
        {
            descRow = cells.Count > 1 ? cells.Skip(1).FirstOrDefault(row => IsRowTagEqual(row, "##")) : null;
        }
        List<Cell> groupRow = cells.Find(IsGroupRow);
        var fields = new Dictionary<string, FieldInfo>();
        foreach (var subTitle in title.SubTitleList)
        {
            if (!DefUtil.IsNormalFieldName(subTitle.Name))
            {
                continue;
            }
            string desc = "";
            if (descRow != null)
            {
                // 如果有子字段,并且子字段个数>=2时,如果对应注释行有效注释个数为1，表示这是给当前字段的注释,
                // 否则是给子字段的注释，取注释为空，而不是第一个注释
                if (subTitle.SubTitles.Count >= 2)
                {
                    int notEmptyCellCount = 0;
                    for (int i = subTitle.FromIndex; i <= subTitle.ToIndex; i++)
                    {
                        if (i >= descRow.Count)
                        {
                            break;
                        }
                        var cellValue = descRow[i].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            ++notEmptyCellCount;
                            desc = cellValue;
                        }
                    }
                    if (notEmptyCellCount > 1)
                    {
                        desc = "";
                    }
                }
                else
                {
                    desc = descRow?[subTitle.FromIndex].Value?.ToString() ?? "";
                }
            }
            fields.Add(subTitle.Name, new FieldInfo()
            {
                Name = subTitle.Name,
                Tags = subTitle.Tags,
                Type = typeRow[subTitle.FromIndex].Value?.ToString() ?? "",
                Groups = groupRow?[subTitle.FromIndex].Value?.ToString() ?? "",
                Desc = desc,
            });
        }

        return new RawSheetTableDefInfo() { FieldInfos = fields };
    }

}
