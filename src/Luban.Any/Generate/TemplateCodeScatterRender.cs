namespace Luban.Any.Generate;

[Render("code_template")]
class TemplateCodeScatterRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => GenerationContext.Ctx.GenArgs.TemplateCodeDir;

    protected override ELanguage GetLanguage(GenerationContext ctx)
    {
        return RenderFileUtil.GetLanguage(ctx.GenArgs.TemplateCodeDir);
    }
}