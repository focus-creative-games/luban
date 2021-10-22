using Luban.Job.Cfg.DataConverts;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    static class DataConvertUtil
    {
        public static object ToConvertRecord(DefTable table, Record record, string converType)
        {
            switch (converType)
            {
                case "convert_json":
                {
                    // data_json与data_json2格式区别在于
                    // data_json的map格式是 [[key1,value1],[] ..]
                    // data_json2的map格式是 { key1:value1, ...}
                    var ss = new MemoryStream();
                    var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
                    {
                        Indented = true,
                        SkipValidation = false,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                    });
                    record.Data.Apply(JsonConvertor.Ins, jsonWriter);
                    jsonWriter.Flush();
                    return System.Text.Encoding.UTF8.GetString(DataUtil.StreamToBytes(ss));
                }
                case "convert_lua":
                {
                    return new LuaConvertor().ExportRecord(table, record);
                }
                //case "data_erlang":
                //{
                //    var content = new StringBuilder();
                //    switch (table.Mode)
                //    {
                //        case ETableMode.ONE:
                //        {
                //            ErlangExport.Ins.ExportTableSingleton(table, records[0], content);
                //            break;
                //        }
                //        case ETableMode.MAP:
                //        {
                //            ErlangExport.Ins.ExportTableMap(table, records, content);
                //            break;
                //        }
                //        default:
                //        {
                //            throw new NotSupportedException();
                //        }
                //    }
                //    return content.ToString();
                //}
                default:
                {
                    throw new ArgumentException($"not support datatype:{converType}");
                }
            }
        }

    }
}
