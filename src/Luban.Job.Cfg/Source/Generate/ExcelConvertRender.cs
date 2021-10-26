using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.DataConverts;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Utils;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("convert_xlsx")]
    [Render("convert_excel")]
    class ExcelConvertRender : DataRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            foreach (var table in ctx.ExportTables)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var records = ctx.Assembly.GetTableAllDataList(table);
                    string dirName = table.FullName;
                    var fileName = table.FullName;
                    var filePath = $"{dirName}/{fileName}.xlsx";


                    var title = TitleCreator.Ins.CreateTitle(table);


                    var dataRangeArray = new List<object[]>();

                    dataRangeArray.Add(new object[] { "##" });

                    var titleRow = new object[title.ToIndex + 1];

                    foreach (var subTitle in title.SubTitleList)
                    {
                        string titleAndTags = subTitle.Tags.Count == 0 ? subTitle.Name : subTitle.Name + "&" + string.Join('&', subTitle.Tags.Select(e => $"{e.Key}={e.Value}"));
                        titleRow[subTitle.FromIndex] = titleAndTags;
                    }
                    dataRangeArray.Add(titleRow);
                    // 注释行1
                    dataRangeArray.Add(Array.Empty<object>());
                    // 注释行2
                    dataRangeArray.Add(Array.Empty<object>());

                    int totalRowCount = dataRangeArray.Count;
                    foreach (var rec in records)
                    {
                        var fillVisitor = new FillSheetVisitor(dataRangeArray, title.ToIndex + 1, totalRowCount);
                        totalRowCount += rec.Data.Apply(fillVisitor, title);
                    }



                    //var memoryStream = new MemoryStream();
                    //using (var package = new ExcelPackage(memoryStream))
                    //{
                    //    var sheet = package.Workbook.Worksheets.Add("sheet1");
                    //    for (int i = 0; i < dataRangeArray.Count; i++)
                    //    {
                    //        var rawRow = dataRangeArray[i];
                    //        sheet.Cells[i + 1, 1, i + 1, rawRow.Length].FillList(rawRow);
                    //    }
                    //    sheet.Cells.AutoFitColumns();
                    //    content = package.GetAsByteArray();
                    //}

                    var worksheet = new SLDocument();
                    //var rows = new List<Row>();
                    //for (int i = 0; i < dataRangeArray.Count; i++)
                    //{
                    //    var rawRow = dataRangeArray[i];
                    //    var cells = new List<Cell>();
                    //    for (int j = 0; j < rawRow.Length; j++)
                    //    {
                    //        cells.Add(new Cell(j + 1, rawRow[j]));
                    //    }
                    //    rows.Add(new Row(i + 1, cells));
                    //}
                    //worksheet.Rows = rows;
                    for (int i = 0; i < dataRangeArray.Count; i++)
                    {
                        var rawRow = dataRangeArray[i];
                        for (int j = 0; j < rawRow.Length; j++)
                        {
                            object v = dataRangeArray[i][j];
                            if (v != null)
                            {
                                switch (v)
                                {
                                    case int t:
                                    {
                                        worksheet.SetCellValue(i + 1, j + 1, t);
                                        break;
                                    }
                                    case string t:
                                    {
                                        worksheet.SetCellValue(i + 1, j + 1, t);
                                        break;
                                    }
                                    case float t:
                                    {
                                        worksheet.SetCellValue(i + 1, j + 1, t);
                                        break;
                                    }
                                    case bool t:
                                    {
                                        worksheet.SetCellValue(i + 1, j + 1, t);
                                        break;
                                    }
                                    case DateTime t:
                                    {

                                        worksheet.SetCellValue(i + 1, j + 1, t);
                                        break;
                                    }
                                    default:
                                    {
                                        worksheet.SetCellValue(i + 1, j + 1, v.ToString());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    worksheet.AutoFitColumn(1, title.ToIndex + 1);
                    worksheet.AutoFitRow(1, dataRangeArray.Count);
                    var stream = new MemoryStream();
                    worksheet.SaveAs(stream);



                    //var tempFile = $"{Path.GetTempFileName()}_{fileName}.tmp";
                    //var outputFile = $"{Path.GetTempFileName()}_{fileName}.xlsx";
                    //var outputStream = new FileStream(tempFile, FileMode.CreateNew, FileAccess.ReadWrite);
                    //var writer = ExcelDataWriter.ExcelDataWriter.GetAsByteArray(dataRangeArray, new ExcelDataWriter.ClassMap<object[]>());
                    //using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(new System.IO.FileInfo(tempFile), new System.IO.FileInfo(outputFile)))
                    //{
                    //    // Write the data
                    //    fastExcel.Write(worksheet, "sheet1");
                    //}
                    //outputStream.Close();
                    //outputStream.Flush();
                    byte[] content = DataUtil.StreamToBytes(stream);
                    var md5 = CacheFileUtil.GenStringOrBytesMd5AndAddCache(filePath, content);
                    FileRecordCacheManager.Ins.AddCachedRecordOutputData(table, records, genType, md5);
                    ctx.GenDataFilesInOutputDataDir.Add(new Luban.Common.Protos.FileInfo() { FilePath = filePath, MD5 = md5 });
                }));

            }
        }
    }
}
