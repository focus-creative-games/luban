using ExcelDataReader;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataSources.Excel
{
    static class SheetLoadUtil
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

        public static IEnumerable<RawSheet> LoadRawSheets(string rawUrl, string sheetName, Stream stream)
        {
            s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
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

            if (!TryParseMeta(reader, out orientRow, out var tableName))
            {
                return null;
            }
            var cells = ParseRawSheetContent(reader, orientRow, false);
            var title = ParseTitle(cells, reader.MergeCells, orientRow);
            cells.RemoveAll(c => c.Count == 0 || IsHeaderRow(c));
            return new RawSheet() { Title = title, TableName = tableName, Cells = cells };
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

            //titleRowNum = GetTitleRowNum(mergeCells, orientRow);

            ParseSubTitles(rootTitle, cells, mergeCells, orientRow, 1);

            rootTitle.Init();

            if (rootTitle.SubTitleList.Count == 0)
            {
                throw new Exception($"没有定义任何有效 列");
            }
            return rootTitle;
        }

        private static bool IsIgnoreTitle(string title)
        {
#if !LUBAN_LITE
            return string.IsNullOrEmpty(title) || title.StartsWith('#');
#else
            return string.IsNullOrEmpty(title) || title.StartsWith("#");
#endif
        }

        public static (string Name, Dictionary<string, string> Tags) ParseNameAndMetaAttrs(string nameAndAttrs)
        {
            var attrs = nameAndAttrs.Split('&');

            string titleName = attrs[0];
            var tags = new Dictionary<string, string>();
            // *  开头的表示是多行
            if (titleName.StartsWith("*"))
            {
                titleName = titleName.Substring(1);
                tags.Add("multi_rows", "1");
            }
            if (titleName.StartsWith("!"))
            {
                titleName = titleName.Substring(1);
                tags.Add("non_empty", "1");
            }
            foreach (var attrPair in attrs.Skip(1))
            {
                var pairs = attrPair.Split('=');
                if (pairs.Length != 2)
                {
                    throw new Exception($"invalid title: {nameAndAttrs}");
                }
                tags.Add(pairs[0], pairs[1]);
            }
            return (titleName, tags);
        }

        private static void ParseSubTitles(Title title, List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow, int curDepth)
        {
            var rowIndex = curDepth - 1;
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
                        if (mergeCell.FromColumn == rowIndex && mergeCell.FromRow - 1 >= title.FromIndex && mergeCell.FromRow - 1 <= title.ToIndex)
                        {
                            // 标题 行
                            var nameAndAttrs = titleRow[mergeCell.FromRow - 1].Value?.ToString()?.Trim();
                            if (IsIgnoreTitle(nameAndAttrs))
                            {
                                continue;
                            }
                            var (titleName, tags) = ParseNameAndMetaAttrs(nameAndAttrs);
                            subTitle = new Title() { Name = titleName, Tags = tags, FromIndex = mergeCell.FromRow - 1, ToIndex = mergeCell.ToRow - 1 };
                        }
                    }
                    if (subTitle == null)
                    {
                        continue;
                    }

                    if (curDepth < cells.Count && IsSubFieldRow(cells[curDepth]))
                    {
                        ParseSubTitles(subTitle, cells, mergeCells, orientRow, curDepth + 1);
                    }
                    title.AddSubTitle(subTitle);

                }
            }

            for (int i = title.FromIndex; i <= title.ToIndex; i++)
            {
                var nameAndAttrs = titleRow[i].Value?.ToString()?.Trim();
                if (IsIgnoreTitle(nameAndAttrs))
                {
                    continue;
                }
                var (titleName, tags) = ParseNameAndMetaAttrs(nameAndAttrs);

                if (title.SubTitles.TryGetValue(titleName, out var subTitle))
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
                if (curDepth < cells.Count && IsSubFieldRow(cells[curDepth]))
                {
                    ParseSubTitles(subTitle, cells, mergeCells, orientRow, curDepth + 1);
                }
                title.AddSubTitle(subTitle);
            }
        }

        public static bool TryParseMeta(string metaStr, out bool orientRow, out string tableName)
        {
            orientRow = true;
            tableName = "";

            // meta 行 必须以 ##为第一个单元格内容,紧接着 key:value 形式 表达meta属性
            if (string.IsNullOrEmpty(metaStr) || !metaStr.StartsWith("##"))
            {
                return false;
            }

            foreach (var attr in metaStr.Substring(2).Split('&'))
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
                    case "row":
                    {
                        orientRow = true;
                        break;
                    }
                    case "column":
                    {
                        orientRow = false;
                        break;
                    }
                    case "table":
                    {
                        tableName = value;
                        break;
                    }
                    default:
                    {
                        throw new Exception($"非法单元薄 meta 属性定义 {attr}, 合法属性有: row,column,table=<tableName>");
                    }
                }
            }
            return true;
        }

        public static bool TryParseMeta(IExcelDataReader reader, out bool orientRow, out string tableName)
        {
            if (!reader.Read() || reader.FieldCount == 0)
            {
                orientRow = true;
                tableName = "";
                return false;
            }
            string metaStr = reader.GetString(0)?.Trim();
            return TryParseMeta(metaStr, out orientRow, out tableName);
        }

        private static bool IsSubFieldRow(List<Cell> row)
        {
            if (row.Count == 0)
            {
                return false;
            }
            var s = row[0].Value?.ToString()?.Trim();
            return s == "##field";
        }

        private static bool IsTypeRow(List<Cell> row)
        {
            if (row.Count == 0)
            {
                return false;
            }
            var s = row[0].Value?.ToString()?.Trim();
            return s == "##type";
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

        private static List<List<Cell>> ParseRawSheetContent(IExcelDataReader reader, bool orientRow, bool headerOnly)
        {
            // TODO 优化性能
            // 几个思路
            // 1. 没有 title 的列不加载
            // 2. 空行优先跳过
            // 3. 跳过null或者empty的单元格
            var originRows = new List<List<Cell>>();
            int rowIndex = 0;
            do
            {
                ++rowIndex; // 第1行是 meta ，标题及数据行从第2行开始
                var row = new List<Cell>();
                for (int i = 0, n = reader.FieldCount; i < n; i++)
                {
                    row.Add(new Cell(rowIndex, i, reader.GetValue(i)));
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

            if (!TryParseMeta(reader, out orientRow, out var _))
            {
                return null;
            }
            var cells = ParseRawSheetContent(reader, orientRow, true);
            var title = ParseTitle(cells, reader.MergeCells, orientRow);

            int typeRowIndex = cells.FindIndex(row => IsTypeRow(row));

            if (typeRowIndex < 0)
            {
                throw new Exception($"缺失type行");
            }
            List<Cell> typeRow = cells[typeRowIndex];
            List<Cell> descRow = cells.Count > typeRowIndex + 1 ? cells[typeRowIndex + 1] : null;

            var fields = new Dictionary<string, FieldInfo>();
            foreach (var subTitle in title.SubTitleList)
            {
                if (!DefUtil.IsNormalFieldName(subTitle.Name))
                {
                    continue;
                }
                fields.Add(subTitle.Name, new FieldInfo()
                {
                    Name = subTitle.Name,
                    Tags = title.Tags,
                    Type = typeRow[subTitle.FromIndex].Value?.ToString() ?? "",
                    Desc = descRow?[subTitle.FromIndex].Value?.ToString() ?? "",
                });
            }

            return new RawSheetTableDefInfo() { FieldInfos = fields };
        }

    }
}
