namespace Luban.Any.Generate;

[Render("data_template")]
class TemplateDataRender : DataRenderBase
{
    public override void Render(GenerationContext ctx)
    {
        string genType = ctx.GenArgs.TemplateDataFile;
        foreach (var table in ctx.ExportTables)
        {
            ctx.Tasks.Add(Task.Run(() =>
            {
                var file = RenderFileUtil.GetOutputFileName(genType, table.OutputDataFile, ctx.GenArgs.OutputDataFileExtension);
                var records = ctx.Assembly.GetTableExportDataList(table);
                if (!FileRecordCacheManager.Ins.TryGetRecordOutputData(table, records, genType, out string md5))
                {
                    var content = DataExporterUtil.ToTemplateOutputData(table, records, genType);
                    md5 = CacheFileUtil.GenStringOrBytesMd5AndAddCache(file, content);
                    FileRecordCacheManager.Ins.AddCachedRecordOutputData(table, records, genType, md5);
                }
                ctx.GenDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }
    }


}