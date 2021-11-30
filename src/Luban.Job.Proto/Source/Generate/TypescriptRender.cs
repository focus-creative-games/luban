using Luban.Common.Protos;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    [Render("typescript")]
    class TypescriptRender : TemplateRenderBase
    {
        public override void Render(GenContext ctx)
        {
            ctx.Tasks.Add(Task.Run(() =>
            {
                GenArgs args = ctx.GenArgs;
                var brightRequirePath = args.TypescriptBrightRequirePath;
                var brightPackageName = args.TypescriptBrightPackageName;
                var render = ctx.Render;
                var ass = ctx.Assembly;
                var fileContent = new List<string>();
                if (args.TypescriptUsePuertsByteBuf)
                {
                    fileContent.Add(TypescriptStringTemplate.PuertsByteBufImports);
                }
                else
                {
                    fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));
                }
                if (args.TypescriptEmbedBrightTypes)
                {
                    fileContent.Add(StringTemplateManager.Ins.GetTemplateString("config/typescript_bin/vectors"));
                    fileContent.Add(TypescriptStringTemplate.SerializeTypes);
                    fileContent.Add(TypescriptStringTemplate.ProtoTypes);
                }
                else
                {
                    fileContent.Add(TypescriptStringTemplate.GetSerializeImports(brightRequirePath, brightPackageName));
                    fileContent.Add(TypescriptStringTemplate.GetProtocolImports(brightRequirePath, brightPackageName));
                    fileContent.Add(TypescriptStringTemplate.GetVectorImports(brightRequirePath, brightPackageName));
                }

                fileContent.Add(@$"export namespace {ass.TopModule} {{");

                foreach (var type in ctx.ExportTypes)
                {
                    fileContent.Add(render.RenderAny(type));
                }

                fileContent.Add(render.RenderStubs("ProtocolStub", ass.TopModule,
                    ctx.ExportTypes.Where(t => t is DefProto).Cast<DefProto>().ToList(),
                    ctx.ExportTypes.Where(t => t is DefRpc).Cast<DefRpc>().ToList()));

                fileContent.Add("}"); // end of topmodule

                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', fileContent), ELanguage.TYPESCRIPT);
                var file = "Types.ts";
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }
    }
}
