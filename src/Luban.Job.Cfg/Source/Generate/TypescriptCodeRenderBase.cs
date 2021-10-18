using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class TypescriptCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            var args = ctx.GenArgs;
            ctx.Render = this;
            ctx.Lan = RenderFileUtil.GetLanguage(genType);

            var lines = new List<string>(10000);
            Action<List<string>> preContent = (fileContent) =>
            {
                var brightRequirePath = args.TypescriptBrightRequirePath;
                var brightPackageName = args.TypescriptBrightPackageName;
                bool isGenBinary = genType.EndsWith("bin");
                if (isGenBinary)
                {
                    if (args.UsePuertsByteBuf)
                    {
                        fileContent.Add(TypescriptStringTemplate.PuertsByteBufImports);
                    }
                    else
                    {
                        fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));
                    }
                }

                if (args.EmbedBrightTypes)
                {
                    fileContent.Add(isGenBinary ?
                        StringTemplateUtil.GetTemplateString("config/typescript_bin/vectors")
                        : StringTemplateUtil.GetTemplateString("config/typescript_json/vectors"));
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

            GenerateCodeMonolithic(ctx, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "Types.ts"), lines, preContent, postContent);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
        }
    }
}
