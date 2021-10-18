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

        private const int TITLE_MIN_ROW_NUM = 2;
        private const int TITLE_MAX_ROW_NUM = 10;
        private const int TITLE_DEFAULT_ROW_NUM = 3;

        private static System.Text.Encoding DetectCsvEncoding(Stream fs)
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
            int titleRowNum;

            if (!TryParseMeta(reader, out orientRow, out titleRowNum, out var tableName))
            {
                return null;
            }
            var cells = ParseRawSheetContent(reader, orientRow);
            var title = ParseTitle(cells, reader.MergeCells, orientRow, out _);
            cells.RemoveRange(0, Math.Min(titleRowNum, cells.Count));
            return new RawSheet() { Title = title, TitleRowCount = titleRowNum, TableName = tableName, Cells = cells };
        }

        private static int GetTitleRowNum(CellRange[] mergeCells, bool orientRow)
        {
            if (mergeCells == null)
            {
                return 1;
            }
            if (orientRow)
            {
                foreach (var mergeCell in mergeCells)
                {
                    if (mergeCell.FromRow == 1 && mergeCell.FromColumn == 0)
                    {
                        return mergeCell.ToRow - mergeCell.FromRow + 1;
                    }
                }
            }
            else
            {
                foreach (var mergeCell in mergeCells)
                {
                    if (mergeCell.FromColumn == 1 && mergeCell.FromRow == 0)
                    {
                        return mergeCell.ToColumn - mergeCell.FromColumn + 1;
                    }
                }
            }
            return 1;
        }

        public static Title ParseTitle(List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow, out int titleRowNum)
        {
            var rootTitle = new Title()
            {
                Root = true,
                Name = "__root__",
                Tags = new Dictionary<string, string>(),
                FromIndex = 0,
                ToIndex = cells.Select(r => r.Count).Max() - 1
            };

            titleRowNum = GetTitleRowNum(mergeCells, orientRow);

            ParseSubTitles(rootTitle, cells, mergeCells, orientRow, 1, titleRowNum);

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
#if !LUBAN_LITE
            if (titleName.StartsWith('*'))
#else
            if (titleName.StartsWith("*"))
#endif
            {
                titleName = titleName.Substring(1);
                tags.Add("multi_rows", "1");
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

        private static void ParseSubTitles(Title title, List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow, int curDepth, int maxDepth)
        {

            var titleRow = cells[curDepth - 1];
            if (mergeCells != null)
            {
                foreach (var mergeCell in mergeCells)
                {
                    Title subTitle = null;
                    if (orientRow)
                    {
                        //if (mergeCell.FromRow <= 1 && mergeCell.ToRow >= 1)
                        if (mergeCell.FromRow == curDepth && mergeCell.FromColumn >= title.FromIndex && mergeCell.FromColumn <= title.ToIndex)
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
                        if (mergeCell.FromColumn == curDepth - 1 && mergeCell.FromRow - 1 >= title.FromIndex && mergeCell.FromRow - 1 <= title.ToIndex)
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

                    if (curDepth < maxDepth)
                    {
                        ParseSubTitles(subTitle, cells, mergeCells, orientRow, curDepth + 1, maxDepth);
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

                if (title.SubTitles.TryGetValue(titleName, out var oldTitle))
                {
                    if (oldTitle.FromIndex != i)
                    {
                        throw new Exception($"列:{titleName} 重复");
                    }
                    else
                    {
                        continue;
                    }
                }
                title.AddSubTitle(new Title() { Name = titleName, Tags = tags, FromIndex = i, ToIndex = i });
            }
        }

        public static bool TryParseMeta(List<string> cells, out bool orientRow, out int titleRows, out string tableName)
        {
            orientRow = true;
            titleRows = TITLE_DEFAULT_ROW_NUM;
            tableName = "";

            // meta 行 必须以 ##为第一个单元格内容,紧接着 key:value 形式 表达meta属性
            if (cells.Count == 0 || cells[0] != "##")
            {
                return false;
            }

            foreach (var attr in cells.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(attr))
                {
                    continue;
                }

                var ss = attr.Split('=');
                if (ss.Length != 2)
                {
                    throw new Exception($"单元薄 meta 定义出错. attribute:{attr}");
                }
                string key = ss[0].Trim();
                string value = ss[1].Trim();
                switch (key)
                {
                    case "orientation":
                    {
                        orientRow = DefUtil.ParseOrientation(value);
                        break;
                    }
                    case "title_rows":
                    {
                        if (!int.TryParse(value, out var v))
                        {
                            throw new Exception($"单元薄 meta 定义 title_rows:{value} 属性值只能为整数[{TITLE_MIN_ROW_NUM},{TITLE_MAX_ROW_NUM}]");
                        }
                        if (v < TITLE_MIN_ROW_NUM || v > TITLE_MAX_ROW_NUM)
                        {
                            throw new Exception($"单元薄 title_rows 应该在 [{TITLE_MIN_ROW_NUM},{TITLE_MAX_ROW_NUM}] 范围内,默认是{TITLE_DEFAULT_ROW_NUM}");
                        }
                        titleRows = v;
                        break;
                    }
                    case "table":
                    {
                        tableName = value;
                        break;
                    }
                    default:
                    {
                        throw new Exception($"非法单元薄 meta 属性定义 {attr}, 合法属性有: orientation=r|row|c|column,title_rows=<number>,table=<tableName>");
                    }
                }
            }
            return true;
        }

        public static bool TryParseMeta(IExcelDataReader reader, out bool orientRow, out int titleRows, out string tableName)
        {
            if (!reader.Read() || reader.FieldCount == 0)
            {
                orientRow = true;
                titleRows = TITLE_DEFAULT_ROW_NUM;
                tableName = "";
                return false;
            }
            var cells = new List<string>();
            for (int i = 0, n = reader.FieldCount; i < n; i++)
            {
                cells.Add(reader.GetString(i)?.Trim());
            }
            return TryParseMeta(cells, out orientRow, out titleRows, out tableName);
        }

        private static List<List<Cell>> ParseRawSheetContent(IExcelDataReader reader, bool orientRow, int? maxParseRow = null)
        {
            // TODO 优化性能
            // 几个思路
            // 1. 没有 title 的列不加载
            // 2. 空行优先跳过
            // 3. 跳过null或者empty的单元格
            var originRows = new List<List<Cell>>();
            int rowIndex = 0;
            while (reader.Read())
            {
                ++rowIndex; // 第1行是 meta ，标题及数据行从第2行开始
                var row = new List<Cell>();
                for (int i = 0, n = reader.FieldCount; i < n; i++)
                {
                    row.Add(new Cell(rowIndex, i, reader.GetValue(i)));
                }
                originRows.Add(row);
                if (orientRow && maxParseRow != null && originRows.Count > maxParseRow)
                {
                    break;
                }
            }

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
            int headerRowNum;

            if (!TryParseMeta(reader, out orientRow, out headerRowNum, out var _))
            {
                return null;
            }
            var cells = ParseRawSheetContent(reader, orientRow, headerRowNum);
            var title = ParseTitle(cells, reader.MergeCells, orientRow, out int titleRowNum);

            if (cells.Count <= titleRowNum)
            {
                throw new Exception($"缺失type行");
            }
            List<Cell> typeRow = cells[titleRowNum];
            List<Cell> briefDescRow = cells.Count > titleRowNum + 1 ? cells[titleRowNum + 1] : null;
            List<Cell> destailDescRow = cells.Count > titleRowNum + 2 ? cells[titleRowNum + 2] : briefDescRow;

            var fields = new Dictionary<string, FieldInfo>();
            foreach (var subTitle in title.SubTitleList)
            {
                fields.Add(subTitle.Name, new FieldInfo()
                {
                    Name = subTitle.Name,
                    Tags = title.Tags,
                    Type = typeRow != null ? typeRow[subTitle.FromIndex].Value?.ToString() : "",
                    BriefDesc = briefDescRow != null ? briefDescRow[subTitle.FromIndex].Value?.ToString() : "",
                    DetailDesc = destailDescRow != null ? destailDescRow[subTitle.FromIndex].Value?.ToString() : "",
                });
            }

            return new RawSheetTableDefInfo() { FieldInfos = fields };
        }

    }
}
