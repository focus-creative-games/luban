using ExcelDataReader;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Luban.Job.Cfg.DataSources.Excel
{

    class ExcelDataSource : AbstractDataSource
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<Sheet> _sheets = new List<Sheet>();


        public override void Load(string rawUrl, string sheetName, Stream stream)
        {
            s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
            RawUrl = rawUrl;


            foreach (RawSheet rawSheet in SheetLoadUtil.LoadRawSheets(rawUrl, sheetName, stream))
            {
                var sheet = new Sheet(rawUrl, sheetName);
                sheet.Load(rawSheet);
                _sheets.Add(sheet);
            }

            if (_sheets.Count == 0)
            {
                throw new Exception($"excel:{rawUrl} 不包含有效的单元薄(有效单元薄的A0单元格必须是##).");
            }
        }

        public void Load(params RawSheet[] rawSheets)
        {
            foreach (RawSheet rawSheet in rawSheets)
            {
                var sheet = new Sheet("__intern__", rawSheet.TableName);
                sheet.Load(rawSheet);
                _sheets.Add(sheet);
            }
        }

        public RawSheetTableDefInfo LoadTableDefInfo(string rawUrl, string sheetName, Stream stream)
        {
            return SheetLoadUtil.LoadSheetTableDefInfo(rawUrl, sheetName, stream);
        }

        public override List<Record> ReadMulti(TBean type)
        {
            var datas = new List<Record>();
            foreach (var sheet in _sheets)
            {
                try
                {
                    foreach (TitleRow row in sheet.GetRows())
                    {
                        var tagRow = row.GetSubTitleNamedRow(TAG_KEY);
                        string tagStr = tagRow?.Current?.ToString();
                        if (DataUtil.IsIgnoreTag(tagStr))
                        {
                            continue;
                        }
                        var data = (DBean)type.Apply(SheetDataCreator.Ins, sheet, row);
                        datas.Add(new Record(data, sheet.RawUrl, DataUtil.ParseTags(tagStr)));
                    }
                }
                catch (DataCreateException dce)
                {
                    dce.OriginDataLocation = sheet.RawUrl;
                    throw;
                }
                catch (Exception e)
                {
                    throw new Exception($"sheet:{sheet.Name}", e);
                }
            }
            return datas;
        }

        public override Record ReadOne(TBean type)
        {
            //var datas = ReadMulti(type);
            //switch (datas.Count)
            //{
            //    case 1: return datas[0];
            //    case 0: throw new Exception($"单例表不能为空，必须包含且只包含1个记录");
            //    default: throw new Exception($"单例表必须恰好包含1个记录. 但当前记录数为:{datas.Count}");
            //}
            throw new Exception($"excel不支持单例读取模式");
        }
    }
}
