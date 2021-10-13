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
        public static Dictionary<string, string> ParseMetaAttrs(Worksheet sheet)
        {
            Range metaRow = sheet.Rows[1];
            if (metaRow.Cells[1, 1].Text.ToString() != "##")
            {
                throw new Exception("A1 should be ##");
            }
            var metaAttrs = new Dictionary<string, string>();
            for (int i = 2, n = sheet.UsedRange.Columns.Count; i <= n; i++)
            {
                Range cell = metaRow.Cells[1, i];
                string value = cell.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var attrs = value.Split('=');
                    if (attrs.Length != 2)
                    {
                        throw new Exception($"invalid meta attr:{value}");
                    }
                    metaAttrs.Add(attrs[0], attrs[1]);
                }
            }
            return metaAttrs;
        }

        public static TitleInfo ParseTitles(Worksheet sheet)
        {

            int titleRows = 1;
            Range c1 = sheet.Cells[2, 1];
            if (c1.MergeCells)
            {
                titleRows = c1.MergeArea.Count;
            }


            var rootTile = new Title()
            {
                FromIndex = 2,
                ToIndex = sheet.UsedRange.Columns.Count,
                Name = "__root__",
                Root = true,
            };
            ParseSubTitle(sheet, 2, titleRows + 1, rootTile);
            return new TitleInfo(rootTile, titleRows);
        }

        private static void ParseSubTitle(Worksheet sheet, int rowIndex, int maxRowIndex, Title title)
        {
            Range row = sheet.Rows[rowIndex];
            for (int i = title.FromIndex; i <= title.ToIndex; i++)
            {
                Range subTitleRange = row.Cells[1, i];
                string subTitleValue = subTitleRange.Value?.ToString();
                if (string.IsNullOrWhiteSpace(subTitleValue))
                {
                    continue;
                }

                var attrs = subTitleValue.Split('&');
                string subTitleName = attrs[0];
                string sep = "";
                foreach (var attrPair in attrs.Skip(1))
                {
                    var pairs = attrPair.Split('=');
                    if (pairs.Length != 2)
                    {
                        throw new Exception($"invalid title: {subTitleValue}");
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
                            throw new Exception($"invalid title: {subTitleValue}");
                        }
                    }
                }

                if (title.SubTitles.ContainsKey(subTitleName))
                {
                    throw new Exception($"title:{subTitleName} 重复");
                }
                var newSubTitle = new Title()
                {
                    Name = subTitleName,
                    FromIndex = i,
                    ToIndex = i,
                };
                if (!string.IsNullOrWhiteSpace(sep))
                {
                    newSubTitle.Sep = sep;
                }
                if (subTitleRange.MergeCells)
                {
                    newSubTitle.ToIndex = i + subTitleRange.MergeArea.Count - 1;
                }
                else
                {
                    newSubTitle.ToIndex = i;
                }
                title.SubTitles.Add(subTitleName, newSubTitle);
            }
            title.SubTitleList.AddRange(title.SubTitles.Values);
            if (rowIndex < maxRowIndex)
            {
                foreach (var subTitle in title.SubTitleList)
                {
                    ParseSubTitle(sheet, rowIndex + 1, maxRowIndex, subTitle);
                }
            }
        }

        public static void FillRecords(Worksheet sheet, Dictionary<string, string> metaAttrs, TitleInfo title, TableDataInfo tableDataInfo)
        {
            int titleRowNum = 3;
            if (metaAttrs.TryGetValue("title_rows", out var titleRowsStr) && !int.TryParse(titleRowsStr, out titleRowNum))
            {
                throw new Exception($"meta 属性 title_rows 不合法");
            }
            if (titleRowNum < title.RowNum)
            {
                throw new Exception($"meta 属性title_rows不能比字段名行的行数小");
            }
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
                nextRowIndex += rec.Data.Apply(fillVisitor, title.RootTitle);
            }
        }

        public static List<Record> LoadRecordsInRange(DefTable table, Worksheet sheet, Title title, Range toSaveRecordRows)
        {
            var recs = new List<Record>();
            foreach (Range row in toSaveRecordRows)
            {
                bool allEmpty = true;
                for (int i = title.FromIndex; i <= title.ToIndex; i++)
                {
                    if (!string.IsNullOrEmpty((row.Cells[1, i] as Range).Value?.ToString()))
                    {
                        allEmpty = false;
                        break;
                    }
                }
                if (allEmpty)
                {
                    continue;
                }
                string tags = (row.Cells[1, 1] as Range).Value?.ToString();
                recs.Add(new Record(
                    (DBean)table.ValueTType.Apply(new SheetDataCreator(sheet, row.Row, table.Assembly), title, null),
                    "",
                    DataUtil.ParseTags(tags)));
            }
            return recs;
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
