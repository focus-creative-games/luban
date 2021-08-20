using Bright.Serialization;
using Luban.Job.Cfg.DataExporters;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.l10n;
using Luban.Job.Cfg.RawDefs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    public static class DataExporterUtil
    {
        public static object ToOutputData(DefTable table, List<Record> records, string dataType)
        {
            switch (dataType)
            {
                case "data_bin":
                {
                    var buf = ThreadLocalTemporalByteBufPool.Alloc(1024 * 1024);
                    BinaryExportor.Ins.WriteList(records, table.Assembly, buf);
                    var bytes = buf.CopyData();
                    ThreadLocalTemporalByteBufPool.Free(buf);
                    return bytes;
                }
                case "data_json":
                case "data_json2":
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
                    if (dataType == "data_json")
                    {
                        JsonExportor.Ins.WriteAsArray(records, table.Assembly, jsonWriter);
                    }
                    else
                    {

                        Json2Exportor.Ins.WriteAsObject(table, records, table.Assembly, jsonWriter);
                    }
                    jsonWriter.Flush();
                    return System.Text.Encoding.UTF8.GetString(DataUtil.StreamToBytes(ss));
                }
                case "data_lua":
                {
                    var content = new List<string>();

                    switch (table.Mode)
                    {
                        case ETableMode.ONE:
                        {
                            LuaExportor.Ins.ExportTableOne(table, records, content);
                            break;
                        }
                        case ETableMode.MAP:
                        {
                            LuaExportor.Ins.ExportTableOneKeyMap(table, records, content);
                            break;
                        }
                        default:
                        {
                            throw new NotSupportedException();
                        }
                    }
                    return string.Join('\n', content);
                }
                default:
                {
                    throw new ArgumentException($"not support datatype:{dataType}");
                }
            }
        }

        public static List<ResourceInfo> ExportResourceList(List<Record> records)
        {
            var resList = new List<ResourceInfo>();
            foreach (Record res in records)
            {
                ResourceExportor.Ins.Accept(res.Data, null, resList);
            }
            return resList;
        }

        public static byte[] GenNotConvertTextList(NotConvertTextSet notConvertSet)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in notConvertSet.SortedEntry)
            {
                sb.Append(e.Key).Append('|').Append(e.Value).Append('\n');
            }
            return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
