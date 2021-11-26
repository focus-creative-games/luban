using Luban.Common.Protos;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    [Render("typescript")]
    class TypescriptRender : RenderBase
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
                if (args.UsePuertsByteBuf)
                {
                    fileContent.Add(TypescriptStringTemplate.PuertsByteBufImports);
                }
                else
                {
                    fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));
                }
                if (args.EmbedBrightTypes)
                {
                    fileContent.Add(StringTemplateUtil.GetTemplateString("config/typescript_bin/vectors"));
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

        protected override string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
        }

        protected override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("proto/typescript/bean");
            var result = template.RenderCode(b);

            return result;
        }

        protected override string Render(DefProto p)
        {
            var template = StringTemplateUtil.GetTemplate("proto/typescript/proto");
            var result = template.RenderCode(p);

            return result;
        }

        protected override string Render(DefRpc r)
        {
            var template = StringTemplateUtil.GetTemplate("proto/typescript/rpc");
            var result = template.RenderCode(r);

            return result;
        }

        public override string RenderStubs(string name, string module, List<DefProto> protos, List<DefRpc> rpcs)
        {
            var template = StringTemplateUtil.GetTemplate("proto/typescript/stub");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Protos = protos,
                Rpcs = rpcs,
            });

            return result;
        }
    }
}
