namespace Luban.Any.Generate;

[Render("code_python2_json")]
class Python2CodeJsonRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => "python2_json";

    public override void Render(GenerationContext ctx)
    {
        ctx.Render = this;
        ctx.Language = Common.ELanguage.PYTHON;
        DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Language;

        var lines = new List<string>(10000);
        static void PreContent(List<string> fileContent)
        {
            //fileContent.Add(PythonStringTemplates.ImportTython3Enum);
            //fileContent.Add(PythonStringTemplates.PythonVectorTypes);
            fileContent.Add(StringTemplateManager.Ins.GetTemplateString("config/python2_json/include"));
        }

        GenerateCodeMonolithic(ctx,
            System.IO.Path.Combine(ctx.GenArgs.OutputCodeDir, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "Types.py")),
            lines,
            PreContent,
            null);

        //string indexFileName = "__init__.py";
        //string indexFileContent = "";
        //var md5 = CacheFileUtil.GenMd5AndAddCache(indexFileName, indexFileContent);
        //ctx.GenScatteredFiles.Add(new FileInfo() { FilePath = indexFileName, MD5 = md5 });
    }
}