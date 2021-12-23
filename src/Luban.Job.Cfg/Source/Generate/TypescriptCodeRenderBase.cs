using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class TypescriptCodeRenderBase : TemplateCodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            var args = ctx.GenArgs;
            ctx.Render = this;
            ctx.Lan = GetLanguage(ctx);
            DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Lan;

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
}
