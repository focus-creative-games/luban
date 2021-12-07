using Luban.Common.Utils;
using Luban.Job.Cfg.DataConverts;
using Luban.Job.Cfg.DataExporters;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LubanAssistant
{
    static class ExcelUtil
    {
        public static RawSheet ParseRawSheet(Worksheet sheet, Range toSaveRecordRows)
        {
            if (!ParseMetaAttrs(sheet, out var orientRow, out var tableName))
            {
                throw new Exception($"meta行不合法");
            }

            if (!orientRow)
            {
                throw new Exception($"目前只支持行表");
            }

            Title title = ParseTitles(sheet);
            var cells = new List<List<Cell>>();

            var rangeAsArray = (object[,])toSaveRecordRows.Value;
            int lowBound0 = rangeAsArray.GetLowerBound(0);
            for (int r = lowBound0, n = r + rangeAsArray.GetLength(0); r < n; r++)
            {
                var rowCell = new List<Cell>();
                for (int i = title.FromIndex; i <= title.ToIndex; i++)
                {
                    rowCell.Add(new Cell(r - 1, i, rangeAsArray[r, i + 1]));
                }
                cells.Add(rowCell);
            }
            return new RawSheet() { Title = title, TableName = tableName, Cells = cells };
        }

        public static RawSheet ParseRawSheetTitleOnly(Worksheet sheet)
        {
            if (!ParseMetaAttrs(sheet, out var orientRow, out var tableName))
            {
                throw new Exception($"meta行不合法");
            }

            if (!orientRow)
            {
                throw new Exception($"目前只支持行表");
            }

            Title title = ParseTitles(sheet);
            var cells = new List<List<Cell>>();
            return new RawSheet() { Title = title, TableName = tableName, Cells = cells };
        }

        public static bool ParseMetaAttrs(Worksheet sheet, out bool orientRow, out string tableName)
        {
            string metaStr = ((Range)sheet.Cells[1, 1]).Value?.ToString();
            return SheetLoadUtil.TryParseMeta(metaStr, out orientRow, out tableName);
        }

        public static Title ParseTitles(Worksheet sheet)
        {
            var rootTile = new Title()
            {
                FromIndex = 0,
                ToIndex = sheet.UsedRange.Columns.Count - 1,
                Name = "__root__",
                Root = true,
                Tags = new Dictionary<string, string>(),
            };
            ParseSubTitle(sheet, 1, rootTile);
            rootTile.ToIndex = rootTile.SubTitleList.Max(t => t.ToIndex);
            rootTile.Init();
            return rootTile;
        }

        private static bool TryParseNextSubFieldRowIndex(Range sheetCells, int startRowIndex, out int rowIndex)
        {
            for (int i = startRowIndex; ; i++)
            {
                string rowTag = sheetCells[i, 1].Value?.ToString() ?? "";
                if (rowTag.StartsWith("##field"))
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

        //private static bool IsSubFieldRow(Range cell)
        //{
        //    var s = cell.Value?.ToString()?.Trim();
        //    return s == "##field";
        //}

        private static bool IsTypeRow(Range cell)
        {
            var s = cell.Value?.ToString()?.Trim();
            return s == "##type";
        }

        private static bool IsHeaderRow(Range cell)
        {
            var s = cell.Value?.ToString()?.Trim();
            return !string.IsNullOrEmpty(s) && s.StartsWith("##");
        }

        private static void ParseSubTitle(Worksheet sheet, int rowIndex, Title title)
        {
            Range row = sheet.Rows[rowIndex];
            for (int i = title.FromIndex; i <= title.ToIndex; i++)
            {
                Range subTitleRange = row.Cells[1, i + 1];
                string subTitleValue = subTitleRange.Value?.ToString();
                if (string.IsNullOrWhiteSpace(subTitleValue))
                {
                    continue;
                }

                var (subTitleName, tags) = SheetLoadUtil.ParseNameAndMetaAttrs(subTitleValue);


                var newSubTitle = new Title()
                {
                    Name = subTitleName,
                    FromIndex = i,
                    Tags = tags,
                };

                if (subTitleRange.MergeCells)
                {
                    newSubTitle.ToIndex = i + subTitleRange.MergeArea.Count - 1;
                }
                else
                {
                    newSubTitle.ToIndex = i;
                }
                title.AddSubTitle(newSubTitle);
            }
            if (rowIndex < sheet.UsedRange.Rows.Count && TryParseNextSubFieldRowIndex(sheet.Cells, rowIndex + 1, out int nextRowIndex))
            {
                foreach (var subTitle in title.SubTitleList)
                {
                    ParseSubTitle(sheet, nextRowIndex, subTitle);
                }
            }
        }

        public static int GetTitleRowCount(Worksheet sheet)
        {
            int firstDataRowIndex = 1;
            while (true)
            {
                string tagCellStr = ((Range)sheet.Cells[firstDataRowIndex, 1]).Value?.ToString();
                if (string.IsNullOrEmpty(tagCellStr) || !tagCellStr.StartsWith("##"))
                {
                    break;
                }
                ++firstDataRowIndex;
            }
            return firstDataRowIndex - 1;
        }

        public static void FillRecords(Worksheet sheet, Title title, TableDataInfo tableDataInfo)
        {
            int usedRowNum = sheet.UsedRange.Rows.Count;
            int firstDataRowIndex = GetTitleRowCount(sheet) + 1;
            if (usedRowNum >= firstDataRowIndex)
            {
                Range allDataRange = sheet.Range[sheet.Cells[firstDataRowIndex, 1], sheet.Cells[usedRowNum, sheet.UsedRange.Columns.Count]];
                allDataRange.ClearContents();
            }

            //int nextRowIndex = titleRowNum + 2;

            var records = DefAssembly.ToSortByKeyDataList(tableDataInfo.Table, tableDataInfo.MainRecords);

            int totalRowCount = 0;
            var dataRangeArray = new List<object[]>();
            foreach (var rec in records)
            {
                var fillVisitor = new FillSheetVisitor(dataRangeArray, title.ToIndex + 1, totalRowCount);
                totalRowCount += rec.Data.Apply(fillVisitor, TBean.Create(false, rec.Data.Type, null), title);
            }

            object[,] resultDataRangeArray = new object[dataRangeArray.Count, title.ToIndex + 1];
            for (int i = 0; i < dataRangeArray.Count; i++)
            {
                object[] row = dataRangeArray[i];
                for (int j = 0; j < row.Length; j++)
                {
                    resultDataRangeArray[i, j] = row[j];
                }
            }

            Range recordFillRange = sheet.Range[sheet.Cells[firstDataRowIndex, 1], sheet.Cells[firstDataRowIndex + dataRangeArray.Count - 1, title.ToIndex + 1]];
            recordFillRange.Value = resultDataRangeArray;
        }

        public static List<Record> LoadRecordsInRange(DefTable table, Worksheet sheet, Title title, Range toSaveRecordRows)
        {
            RawSheet rawSheet = ParseRawSheet(sheet, toSaveRecordRows);
            var excelSource = new ExcelDataSource();
            excelSource.Load(rawSheet);

            return excelSource.ReadMulti(table.ValueTType);
        }

        public static async Task SaveRecordsAsync(string inputDataDir, DefTable table, List<Record> records)
        {
            var recordOutputDir = Path.Combine(inputDataDir, table.InputFiles[0]);
            string index = table.IndexField.Name;

            var saveRecordTasks = new List<Task>();

            foreach (var r in records)
            {
                saveRecordTasks.Add(Task.Run(async () =>
                {
                    var ss = new MemoryStream();
                    var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
                    {
                        Indented = true,
                        SkipValidation = false,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                    });
                    RawJsonExportor.Ins.Accept(r.Data, jsonWriter);

                    jsonWriter.Flush();
                    byte[] resultBytes = DataUtil.StreamToBytes(ss);
                    var key = r.Data.GetField(index);
                    var fileName = $"{key.Apply(ToStringVisitor.Ins)}.json";

                    // 只有文件内容改变才重新加载
                    string fileFullPath = Path.Combine(recordOutputDir, fileName);
                    if (File.Exists(fileFullPath))
                    {
                        var oldBytes = await FileUtil.ReadAllBytesAsync(fileFullPath);
                        if (System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(resultBytes, oldBytes))
                        {
                            return;
                        }
                    }
                    await FileUtil.SaveFileAsync(recordOutputDir, fileName, resultBytes);
                }));
            }
            await Task.WhenAll(saveRecordTasks);
        }

        //public static void FillRecord(Worksheet sheet, ref int nextRowIndex, Title title, Record record)
        //{

        //    nextRowIndex += FillField(sheet, nextRowIndex, title, record.Data);
        //}
    }
}
