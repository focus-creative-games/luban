namespace Luban.Any.Generate;

abstract class ProtobufSchemaRenderBase : TemplateCodeRenderBase
{
    public override void Render(GenerationContext ctx)
    {
        DefAssembly.LocalAssebmly.CurrentLanguage = Common.ELanguage.PROTOBUF;
        var file = RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "schema.proto");
        var content = this.RenderAll(ctx.ExportTypes);
        var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
        ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
    }
}