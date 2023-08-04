namespace Luban.Any.Generate;

[Render("code_rust_json")]
class RustCodeJsonRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => "rust_json";

    public override void Render(GenerationContext ctx)
    {
        string genType = ctx.GenType;
        var args = ctx.GenArgs;
        ctx.Render = this;
        ctx.Language = GetLanguage(ctx);
        DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Language;

        var lines = new List<string>();
        GenerateCodeMonolithic(ctx,
            System.IO.Path.Combine(ctx.GenArgs.OutputCodeDir, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "mod.rs")),
            lines,
            ls =>
            {
                var template = GetConfigTemplate("include");
                var result = template.RenderCode(ctx.ExportTypes);
                ls.Add(result);
            }, null);
    }
}