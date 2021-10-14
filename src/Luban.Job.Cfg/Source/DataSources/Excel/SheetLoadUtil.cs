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

        //private bool IsOrientRow { get; set; } = true; //  以行为数据读取方向

        //public int HeaderRowCount { get; private set; } = TITLE_DEFAULT_ROW_NUM; // 默认有三行是标题行. 第一行是字段名，第二行是中文描述，第三行是注释

        //public int AttrRowCount { get; private set; }

        //public string RawUrl { get; }

        //public string Name { get; }

        //private List<List<Cell>> _rowColumns;

        //private Title _rootTitle;

        //public List<Title> RootFields => _rootTitle.SubTitleList;

        //public List<List<Cell>> RowColumns => _rowColumns;

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

            if (!TryParseMeta(reader, out orientRow, out titleRowNum, out var _))
            {
                return null;
            }
            var cells = ParseRawSheetContent(reader, orientRow);
            var title = ParseTitle(cells, reader.MergeCells, orientRow);
            cells.RemoveRange(0, Math.Min(titleRowNum, cells.Count));
            return new RawSheet() { Title = title, Cells = cells };
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

        public static Title ParseTitle(List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow)
        {
            var rootTitle = new Title() { Root = true, Name = "__root__", FromIndex = 0, ToIndex = cells.Select(r => r.Count).Max() - 1 };

            int titleRowNum = GetTitleRowNum(mergeCells, orientRow);

            ParseSubTitles(rootTitle, cells, mergeCells, orientRow, 1, titleRowNum);

            rootTitle.SortSubTitles();

            if (rootTitle.SubTitleList.Count == 0)
            {
                throw new Exception($"没有定义任何有效 列");
            }
            return rootTitle;
        }

        private static bool IsIgnoreTitle(string title)
        {
            return string.IsNullOrEmpty(title) || title.StartsWith('#');
        }

        private static (string Name, string Sep) ParseNameAndMetaAttrs(string nameAndAttrs)
        {
            var attrs = nameAndAttrs.Split('&');

            string titleName = attrs[0];
            string sep = "";
            foreach (var attrPair in attrs.Skip(1))
            {
                var pairs = attrPair.Split('=');
                if (pairs.Length != 2)
                {
                    throw new Exception($"invalid title: {nameAndAttrs}");
                }
                switch (pairs[0])
                {
                    case "sep":
                    {
                        sep = pairs[1];
                        break;
                    }
                    default:
                    {
                        throw new Exception($"invalid title: {nameAndAttrs}");
                    }
                }
            }
            return (titleName, sep);
        }

        private static void ParseSubTitles(Title title, List<List<Cell>> cells, CellRange[] mergeCells, bool orientRow, int curDepth, int maxDepth)
        {

            var titleRow = cells[curDepth - 1];
            foreach (var mergeCell in mergeCells)
            {
                Title subTitle = null;
                if (orientRow)
                {
                    //if (mergeCell.FromRow <= 1 && mergeCell.ToRow >= 1)
                    if (mergeCell.FromRow == curDepth)
                    {
                        var nameAndAttrs = titleRow[mergeCell.FromColumn].Value?.ToString()?.Trim();
                        if (IsIgnoreTitle(nameAndAttrs))
                        {
                            continue;
                        }
                        var (titleName, sep) = ParseNameAndMetaAttrs(nameAndAttrs);
                        subTitle = new Title() { Name = titleName, Sep = sep, FromIndex = mergeCell.FromColumn, ToIndex = mergeCell.ToColumn };
                        //s_logger.Info("=== sheet:{sheet} title:{title}", Name, newTitle);
                    }
                }
                else
                {
                    if (mergeCell.FromColumn == curDepth - 1)
                    {
                        // 标题 行
                        var nameAndAttrs = titleRow[mergeCell.FromRow - 1].Value?.ToString()?.Trim();
                        if (IsIgnoreTitle(nameAndAttrs))
                        {
                            continue;
                        }
                        var (titleName, sep) = ParseNameAndMetaAttrs(nameAndAttrs);
                        subTitle = new Title() { Name = titleName, Sep = sep, FromIndex = mergeCell.FromRow - 1, ToIndex = mergeCell.ToRow - 1 };
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

            for (int i = 0; i < titleRow.Count; i++)
            {
                var nameAndAttrs = titleRow[i].Value?.ToString()?.Trim();
                if (IsIgnoreTitle(nameAndAttrs))
                {
                    continue;
                }
                var (titleName, sep) = ParseNameAndMetaAttrs(nameAndAttrs);

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
                title.AddSubTitle(new Title() { Name = titleName, Sep = sep, FromIndex = i, ToIndex = i });
            }
        }


        public static RawSheetTableDefInfo LoadSheetTableDefInfo(string rawUrl, string sheetName, Stream stream)
        {
            s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
            string ext = Path.GetExtension(rawUrl);
            //using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(stream) : ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration() { FallbackEncoding = DetectCsvEncoding(stream) }))
            //{
            //    do
            //    {
            //        if (sheetName == null || reader.Name == sheetName)
            //        {
            //            try
            //            {
            //                var sheet = ReadSheet(rawUrl, reader);
            //                if (sheet != null)
            //                {
            //                    _sheets.Add(sheet);
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                throw new Exception($"excel:{rawUrl} sheet:{reader.Name} 读取失败.", e);
            //            }

            //        }
            //    } while (reader.NextResult());
            //}
            return null;
        }

        public static bool TryParseMeta(IExcelDataReader reader, out bool orientRow, out int titleRows, out string tableName)
        {
            orientRow = true;
            titleRows = TITLE_DEFAULT_ROW_NUM;
            tableName = "";
            if (!reader.Read() || reader.FieldCount == 0)
            {
                return false;
            }
            // meta 行 必须以 ##为第一个单元格内容,紧接着 key:value 形式 表达meta属性
            if (reader.GetString(0) != "##")
            {
                return false;
            }

            for (int i = 1, n = reader.FieldCount; i < n; i++)
            {
                var attr = reader.GetString(i)?.Trim();
                if (string.IsNullOrWhiteSpace(attr))
                {
                    continue;
                }

                var ss = attr.Split('=');
                if (ss.Length != 2)
                {
                    throw new Exception($"单元薄 meta 定义出错. attribute:{attr}");
                }
                string key = ss[0].Trim().ToLower();
                string value = ss[1].Trim().ToLower();
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

        private static List<List<Cell>> ParseRawSheetContent(IExcelDataReader reader, bool orientRow)
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



        private static bool IsBlankRow(List<Cell> row)
        {
            // 第一列被策划用于表示是否注释掉此行
            // 忽略此列是否空白
            return row.GetRange(1, row.Count - 1).All(c => c.Value == null || (c.Value is string s && string.IsNullOrWhiteSpace(s)));
        }

        private static bool IsBlankRow(List<Cell> row, int fromIndex, int toIndex)
        {
            for (int i = Math.Max(1, fromIndex), n = Math.Min(toIndex, row.Count - 1); i <= n; i++)
            {
                var v = row[i].Value;
                if (v != null && !(v is string s && string.IsNullOrEmpty(s)))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsSameRow(List<Cell> row1, List<Cell> row2, int fromIndex, int toIndex)
        {
            if (row2.Count < toIndex - 1)
            {
                return false;
            }
            for (int i = Math.Max(1, fromIndex), n = Math.Min(toIndex, row1.Count - 1); i <= n; i++)
            {
                var v1 = row1[i].Value;
                var v2 = row2[i].Value;
                if (v1 != v2)
                {
                    if (v1 == null)
                    {
                        if (!(v2 is string s && string.IsNullOrWhiteSpace(s)))
                        {
                            return false;
                        }
                    }
                    else if (v2 == null)
                    {
                        if (!(v1 is string s && string.IsNullOrWhiteSpace(s)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return v1.ToString() == v2.ToString();
                    }
                }
            }
            return true;
        }

        private static bool IsBlankColumn(List<List<Cell>> rows, int column)
        {
            foreach (List<Cell> row in rows)
            {
                if (column >= row.Count)
                {
                    continue;
                }
                var v = row[column].Value;
                if (v != null && !(v is string s && string.IsNullOrEmpty(s)))
                {
                    return false;
                }
            }
            return true;
        }



    }
}
