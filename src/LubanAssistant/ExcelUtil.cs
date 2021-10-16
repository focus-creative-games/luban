using Luban.Job.Cfg.DataExporters;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LubanAssistant
{
    static class ExcelUtil
    {
        public static RawSheet ParseRawSheet(Worksheet sheet, Range toSaveRecordRows)
        {
            if (!ParseMetaAttrs(sheet, out var orientRow, out var titleRows, out var tableName))
            {
                throw new Exception($"meta行不合法");
            }

            if (!orientRow)
            {
                throw new Exception($"目前只支持行表");
            }

            Title title = ParseTitles(sheet);
            var cells = new List<List<Cell>>();

            foreach (Range row in toSaveRecordRows)
            {
                var rowCell = new List<Cell>();
                for (int i = title.FromIndex; i <= title.ToIndex; i++)
                {
                    rowCell.Add(new Cell(row.Row - 1, i, (row.Cells[1, i + 1] as Range).Value));
                }
                cells.Add(rowCell);
            }
            return new RawSheet() { Title = title, TitleRowCount = titleRows, TableName = tableName, Cells = cells };
        }

        public static RawSheet ParseRawSheetTitleOnly(Worksheet sheet)
        {
            if (!ParseMetaAttrs(sheet, out var orientRow, out var titleRows, out var tableName))
            {
                throw new Exception($"meta行不合法");
            }

            if (!orientRow)
            {
                throw new Exception($"目前只支持行表");
            }

            Title title = ParseTitles(sheet);
            var cells = new List<List<Cell>>();
            return new RawSheet() { Title = title, TitleRowCount = titleRows, TableName = tableName, Cells = cells };
        }

        public static bool ParseMetaAttrs(Worksheet sheet, out bool orientRow, out int titleRows, out string tableName)
        {
            Range metaRow = sheet.Rows[1];

            var cells = new List<string>();
            for (int i = 1, n = sheet.UsedRange.Columns.Count; i <= n; i++)
            {
                cells.Add(((Range)metaRow.Cells[1, i]).Value?.ToString());
            }
            return SheetLoadUtil.TryParseMeta(cells, out orientRow, out titleRows, out tableName);
        }

        public static Title ParseTitles(Worksheet sheet)
        {
            int titleRows = 1;
            Range c1 = sheet.Cells[2, 1];
            if (c1.MergeCells)
            {
                titleRows = c1.MergeArea.Count;
            }
            var rootTile = new Title()
            {
                FromIndex = 0,
                ToIndex = sheet.UsedRange.Columns.Count - 1,
                Name = "__root__",
                Root = true,
                Tags = new Dictionary<string, string>(),
            };
            ParseSubTitle(sheet, 2, titleRows + 1, rootTile);
            rootTile.Init();
            return rootTile;
        }

        private static void ParseSubTitle(Worksheet sheet, int rowIndex, int maxRowIndex, Title title)
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
            if (rowIndex < maxRowIndex)
            {
                foreach (var subTitle in title.SubTitleList)
                {
                    ParseSubTitle(sheet, rowIndex + 1, maxRowIndex, subTitle);
                }
            }
        }

        public static void FillRecords(Worksheet sheet, int titleRowNum, Title title, TableDataInfo tableDataInfo)
        {
            int usedRowNum = sheet.UsedRange.Rows.Count;
            if (usedRowNum > titleRowNum + 1)
            {
                Range allDataRange = sheet.Range[$"A{titleRowNum + 2},A{usedRowNum}"].EntireRow;
                allDataRange.ClearContents();
            }

            int nextRowIndex = titleRowNum + 2;

            foreach (var rec in tableDataInfo.MainRecords)
            {
                var fillVisitor = new FillSheetVisitor(sheet, nextRowIndex);
                //FillRecord(sheet, ref nextRowIndex, title.RootTitle, rec);
                nextRowIndex += rec.Data.Apply(fillVisitor, title);
            }
        }

        public static List<Record> LoadRecordsInRange(DefTable table, Worksheet sheet, Title title, Range toSaveRecordRows)
        {
            RawSheet rawSheet = ParseRawSheet(sheet, toSaveRecordRows);
            var excelSource = new ExcelDataSource();
            excelSource.Load(rawSheet);

            return excelSource.ReadMulti(table.ValueTType);
        }

        public static void SaveRecords(string inputDataDir, DefTable table, List<Record> records)
        {
            var recordOutputDir = Path.Combine(inputDataDir, table.InputFiles[0]);
            string index = table.IndexField.Name;
            foreach (var r in records)
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
                var key = r.Data.GetField(index);
                var fileName = $"{key.Apply(ToStringVisitor.Ins)}.json";
                File.WriteAllBytes(Path.Combine(recordOutputDir, fileName), DataUtil.StreamToBytes(ss));
            }
        }

        //public static void FillRecord(Worksheet sheet, ref int nextRowIndex, Title title, Record record)
        //{

        //    nextRowIndex += FillField(sheet, nextRowIndex, title, record.Data);
        //}
    }
}
