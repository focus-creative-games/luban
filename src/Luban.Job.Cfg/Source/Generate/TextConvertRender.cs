using Luban.Common.Protos;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Utils;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("convert_json")]
    [Render("convert_lua")]
    class TextConvertRender : DataRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            foreach (var table in ctx.ExportTables)
            {
                var records = ctx.Assembly.GetTableAllDataList(table);
                int index = 0;
                string dirName = table.FullName;
                foreach (var record in records)
                {
                    var fileName = table.IsMapTable ?
                        record.Data.GetField(table.IndexField.Name).ToString().Replace("\"", "").Replace("'", "")
                        : (++index).ToString();
                    var file = RenderFileUtil.GetOutputFileName(genType, $"{dirName}/{fileName}", ctx.GenArgs.DataFileExtension);
                    ctx.Tasks.Add(Task.Run(() =>
                    {
                        //if (!FileRecordCacheManager.Ins.TryGetRecordOutputData(table, records, genType, out string md5))
                        //{
                        var content = DataConvertUtil.ToConvertRecord(table, record, genType);
                        var md5 = CacheFileUtil.GenStringOrBytesMd5AndAddCache(file, content);
                        FileRecordCacheManager.Ins.AddCachedRecordOutputData(table, records, genType, md5);
                        //}
                        ctx.GenDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                    }));
                }
            }
        }
    }
}
