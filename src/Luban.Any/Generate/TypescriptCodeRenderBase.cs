namespace Luban.Any.Generate;

abstract class TypescriptCodeRenderBase : TemplateCodeRenderBase
{
    public override void Render(GenerationContext ctx)
    {
        string genType = ctx.GenType;
        var args = ctx.GenArgs;
        ctx.Render = this;
        ctx.Language = GetLanguage(ctx);
        DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Language;

        var lines = new List<string>(10000);
        Action<List<string>> preContent = (fileContent) =>
        {
            var brightRequirePath = args.TypescriptBrightRequirePath;
            var brightPackageName = args.TypescriptBrightPackageName;
            bool isGenBinary = genType.EndsWith("bin");
            if (isGenBinary)
            {
                if (args.TypescriptUsePuertsByteBuf)
                {
                    fileContent.Add(TypescriptStringTemplate.PuertsByteBufImports);
                }
                else
                {
                    fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));
                }
            }

            if (args.TypescriptEmbedBrightTypes)
            {
                fileContent.Add(isGenBinary ?
                    StringTemplateManager.Ins.GetTemplateString("config/typescript_bin/vectors")
                    : StringTemplateManager.Ins.GetTemplateString("config/typescript_json/vectors"));
                if (isGenBinary)
                {
                    fileContent.Add(TypescriptStringTemplate.SerializeTypes);
                }
            }
            else
            {
                if (isGenBinary)
                {
                    fileContent.Add(TypescriptStringTemplate.GetSerializeImports(brightRequirePath, brightPackageName));
                }
                fileContent.Add(TypescriptStringTemplate.GetVectorImports(brightRequirePath, brightPackageName));
            }

            //fileContent.Add(@$"export namespace {ctx.TopModule} {{");
        };

        Action<List<string>> postContent = (fileContent) =>
        {
            //fileContent.Add("}"); // end of topmodule
        };

        GenerateCodeMonolithic(ctx,
            System.IO.Path.Combine(ctx.GenArgs.OutputCodeDir, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "Types.ts")),
            lines,
            preContent,
            postContent);
    }
}