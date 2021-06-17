using Bright.Serialization;
using Luban.Config.Common.RawDefs;
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
        public static byte[] ToOutputData(DefTable table, List<Record> records, string dataType)
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
                {
                    var ss = new MemoryStream();
                    var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
                    {
                        Indented = true,
                        SkipValidation = false,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                    });
                    JsonExportor.Ins.WriteList(records, table.Assembly, jsonWriter);
                    jsonWriter.Flush();
                    return DataUtil.StreamToBytes(ss);
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
                        case ETableMode.BMAP:
                        {
                            LuaExportor.Ins.ExportTableTwoKeyMap(table, records, content);
                            break;
                        }
                        default:
                        {
                            throw new NotSupportedException();
                        }
                    }
                    return System.Text.Encoding.UTF8.GetBytes(string.Join('\n', content));
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
