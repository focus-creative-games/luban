using Luban.Common.Protos;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("data_bin")]
    [Render("data_bidx")]
    [Render("data_json")]
    [Render("data_json2")]
    [Render("data_bson")]
    [Render("data_lua")]
    [Render("data_xml")]
    [Render("data_yaml")]
    [Render("data_protobuf_bin")]
    [Render("data_protobuf_json")]
    [Render("data_msgpack")]
    [Render("data_flatbuffers_json")]
    class DataScatterRender : DataRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            foreach (var table in ctx.ExportTables)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var file = RenderFileUtil.GetOutputFileName(genType, table.OutputDataFile, ctx.GenArgs.OutputDataFileExtension);
                    var records = ctx.Assembly.GetTableExportDataList(table);
                    if (!FileRecordCacheManager.Ins.TryGetRecordOutputData(table, records, genType, out string md5))
                    {
                        var content = DataExporterUtil.ToOutputData(table, records, genType);
                        md5 = CacheFileUtil.GenStringOrBytesMd5AndAddCache(file, content);
                        FileRecordCacheManager.Ins.AddCachedRecordOutputData(table, records, genType, md5);
                    }
                    ctx.GenDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }
    }
}
