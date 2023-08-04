namespace Luban.Any.Generate;

[Render("code_cpp_ue_bp")]
class CppUE4BpRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => "cpp_ue_bp";

    public override void Render(GenerationContext ctx)
    {
        foreach (var c in ctx.ExportTypes)
        {
            if (!(c is DefEnum || c is DefBean))
            {
                continue;
            }

            ctx.Tasks.Add(Task.Run(() =>
            {
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(RenderAny(c), ELanguage.CPP);
                var file = "bp_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }
    }

    public override string Render(DefEnum e)
    {
        var template = GetConfigTemplate("enum");
        var result = template.Render(e);
        return result;
    }
}