namespace Luban.Any.Generate;

abstract class TemplateEditorJsonCodeRenderBase :TemplateCodeRenderBase
{
    public override string Render(DefEnum e)
    {
        var template = GetConfigTemplate("enum");
        var result = template.RenderCode(e);
        return result;
    }

    public override void Render(GenerationContext ctx)
    {
        ELanguage lan = GetLanguage(ctx);
        ctx.Assembly.CurrentLanguage = lan;
        foreach (var c in ctx.Assembly.Types.Values)
        {
            if (c is not DefBean && c is not DefEnum)
            {
                continue;
            }
            ctx.Tasks.Add(Task.Run(() =>
            {
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(RenderAny(c), lan);
                var file = RenderFileUtil.GetDefTypePath(c.FullName, lan);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }
    }
}