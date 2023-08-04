namespace Luban.Any.Generate;

[Render("code_gdscript_json")]
internal class GDScriptCodeJsonRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => "gdscript_json";

    public override void Render(GenerationContext ctx)
    {
        ctx.Render = this;
        ctx.Language = Common.ELanguage.GDSCRIPT;
        DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Language;

        var lines = new List<string>(10000);
        static void PreContent(List<string> fileContent)
        {
            //fileContent.Add(PythonStringTemplates.ImportTython3Enum);
            //fileContent.Add(PythonStringTemplates.PythonVectorTypes);
            fileContent.Add(StringTemplateManager.Ins.GetTemplateString("config/gdscript_json/header"));
        }

        GenerateCodeMonolithic(ctx,
            System.IO.Path.Combine(ctx.GenArgs.OutputCodeDir, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "types.gd")),
            lines,
            PreContent,
            null);
    }
}