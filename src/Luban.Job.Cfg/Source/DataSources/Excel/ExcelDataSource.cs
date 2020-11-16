using ExcelDataReader;
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
            string ext = Path.GetExtension(rawUrl);
            using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(stream) : ExcelReaderFactory.CreateCsvReader(stream))
            {
                do
                {
                    if (sheetName == null || reader.Name == sheetName)
                    {
                        try
                        {
                            var sheet = ReadSheet(reader, exportTestData);
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

        private Sheet ReadSheet(IExcelDataReader reader, bool exportTestData)
        {
            var sheet = new Sheet(reader.Name ?? "", exportTestData);
            return sheet.Load(reader) ? sheet : null;
        }

        public override List<DType> ReadMulti(TBean type)
        {
            var datas = new List<DType>();
            foreach (var sheet in _sheets)
            {
                try
                {
                    datas.AddRange(sheet.ReadMulti(type, ((DefBean)type.Bean).IsMultiRow));
                }
                catch (Exception e)
                {
                    throw new Exception($"sheet:{sheet.Name} ==> {e.Message} {e.StackTrace}", e);
                }
            }
            return datas;
        }

        public override DType ReadOne(TBean type)
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
