using ExcelDataReader;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.Excel
{

    class ExcelDataSource : AbstractDataSource
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<Sheet> _sheets = new List<Sheet>();



        public override void Load(string rawUrl, string sheetName, Stream stream, bool exportTestData)
        {
            s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
            RawUrl = rawUrl;
            string ext = Path.GetExtension(rawUrl);
            using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(stream) : ExcelReaderFactory.CreateCsvReader(stream))
            {
                do
                {
                    if (sheetName == null || reader.Name == sheetName)
                    {
                        try
                        {
                            var sheet = ReadSheet(rawUrl, reader);
                            if (sheet != null)
                            {
                                _sheets.Add(sheet);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"excel:{rawUrl} sheet:{reader.Name} 读取失败. ==> {e.Message}", e);
                        }

                    }
                } while (reader.NextResult());
            }
            if (_sheets.Count == 0)
            {
                throw new Exception($"excel:{rawUrl} 不包含有效的单元薄(有效单元薄的A0单元格必须是##).");
            }
        }

        public Sheet LoadFirstSheet(string rawUrl, string sheetName, Stream stream)
        {
            s_logger.Trace("{filename} {sheet}", rawUrl, sheetName);
            RawUrl = rawUrl;
            string ext = Path.GetExtension(rawUrl);
            using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(stream) : ExcelReaderFactory.CreateCsvReader(stream))
            {
                do
                {
                    if (sheetName == null || reader.Name == sheetName)
                    {
                        try
                        {
                            var sheet = ReadSheetHeader(rawUrl, reader);
                            if (sheet != null)
                            {
                                return sheet;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"excel:{rawUrl} sheet:{reader.Name} 读取失败. ==> {e.Message}", e);
                        }

                    }
                } while (reader.NextResult());
            }
            throw new Exception($"excel:{rawUrl} 不包含有效的单元薄(有效单元薄的A0单元格必须是##).");
        }

        private Sheet ReadSheet(string url, IExcelDataReader reader)
        {
            var sheet = new Sheet(url, reader.Name ?? "");
            return sheet.Load(reader, false) ? sheet : null;
        }

        private Sheet ReadSheetHeader(string url, IExcelDataReader reader)
        {
            var sheet = new Sheet(url, reader.Name ?? "");
            return sheet.Load(reader, true) ? sheet : null;
        }

        public override List<Record> ReadMulti(TBean type)
        {
            var datas = new List<Record>();
            foreach (var sheet in _sheets)
            {
                try
                {
                    datas.AddRange(sheet.ReadMulti(type, ((DefBean)type.Bean).IsMultiRow));
                }
                catch (DataCreateException dce)
                {
                    dce.OriginDataLocation = sheet.RawUrl;
                    throw;
                }
                catch (Exception e)
                {
                    throw new Exception($"sheet:{sheet.Name} ==> {e.Message} {e.StackTrace}", e);
                }
            }
            return datas;
        }

        public override Record ReadOne(TBean type)
        {
            var datas = ReadMulti(type);
            switch (datas.Count)
            {
                case 1: return datas[0];
                case 0: throw new Exception($"单例表不能为空，必须包含且只包含1个记录");
                default: throw new Exception($"单例表必须恰好包含1个记录. 但当前记录数为:{datas.Count}");
            }
        }
    }
}
