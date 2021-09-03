using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_typescript_editor_json")]
    class TypescriptEditorCodeJsonRender : TypescriptCodeRenderBase
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
                fileContent.Add(StringTemplateUtil.GetTemplateString("config/typescript_json/vectors"));

                fileContent.Add(@$"export namespace {ctx.TopModule} {{");
            };

            Action<List<string>> postContent = (fileContent) =>
            {
                fileContent.Add("}\n"); // end of topmodule
            };

            GenerateCode(ctx, "Types.ts", lines, preContent, postContent);
        }

        protected void GenerateCode(GenContext ctx, string outputFile, List<string> fileContent, Action<List<string>> preContent, Action<List<string>> postContent)
        {
            ctx.Tasks.Add(Task.Run(() =>
            {
                fileContent.Add(FileHeaderUtil.GetAutoGenerationHeader(ctx.Lan));

                preContent?.Invoke(fileContent);

                foreach (var c in ctx.Assembly.Types.Values)
                {
                    switch (c)
                    {
                        case DefConst:
                        case DefEnum:
                        case DefBean:
                        case DefTable:
                        {
                            fileContent.Add(ctx.Render.RenderAny(c));
                            break;
                        }
                    }
                }
                postContent?.Invoke(fileContent);

                var file = outputFile;
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', fileContent));
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        public override string Render(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate("config/typescript_editor_json/enum");
            var result = template.RenderCode(e);

            return result;
        }

        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/typescript_editor_json/bean");
            var result = template.RenderCode(b);
            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/typescript_editor_json/table");
            var result = template.RenderCode(p);
            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            throw new NotSupportedException();
        }
    }
}
