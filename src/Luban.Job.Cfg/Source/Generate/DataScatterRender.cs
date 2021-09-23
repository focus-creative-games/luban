using Luban.Common.Protos;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Utils;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("data_bin")]
    [Render("data_json")]
    [Render("data_json2")]
    [Render("data_lua")]
    [Render("data_xml")]
    [Render("data_yaml")]
    class DataScatterRender : DataRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            foreach (var table in ctx.ExportTables)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var file = RenderFileUtil.GetOutputFileName(genType, table.OutputDataFile, ctx.GenArgs.DataFileExtension);
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
