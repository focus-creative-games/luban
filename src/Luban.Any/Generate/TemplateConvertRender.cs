namespace Luban.Any.Generate;

[Render("convert_template")]
class TemplateConvertRender : DataRenderBase
{
    protected Template GetConvertTemplate(string name)
    {
        return StringTemplateManager.Ins.GetTemplate($"config/convert/{name}");
    }

    public override void Render(GenerationContext ctx)
    {
        string genType = ctx.GenType;

        Template template = GetConvertTemplate(ctx.GenArgs.TemplateConvertFile);

        foreach (var table in ctx.ExportTables)
        {
            var records = ctx.Assembly.GetTableAllDataList(table);
            int index = 0;
            string dirName = table.FullName;
            foreach (var record in records)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var fileName = table.IsMapTable ?
                        record.Data.GetField(table.IndexField.Name).Apply(ToStringVisitor2.Ins).Replace("\"", "").Replace("'", "")
                        : (++index).ToString();
                    var file = RenderFileUtil.GetOutputFileName(genType, $"{dirName}/{fileName}", ctx.GenArgs.OutputConvertFileExtension);
                    var content = template.RenderData(table, record.Data);
                    var md5 = CacheFileUtil.GenStringOrBytesMd5AndAddCache(file, content);
                    FileRecordCacheManager.Ins.AddCachedRecordOutputData(table, records, genType, md5);
                    ctx.GenDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }
    }
}