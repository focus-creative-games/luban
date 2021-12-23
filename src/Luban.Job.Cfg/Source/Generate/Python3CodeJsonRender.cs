using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_python3_json")]
    class Python3CodeJsonRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "python3_json";

        public override void Render(GenContext ctx)
        {
            ctx.Render = this;
            ctx.Lan = Common.ELanguage.PYTHON;
            DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Lan;

            var lines = new List<string>(10000);
            static void PreContent(List<string> fileContent)
            {
                //fileContent.Add(PythonStringTemplates.ImportTython3Enum);
                //fileContent.Add(PythonStringTemplates.PythonVectorTypes);
                fileContent.Add(StringTemplateManager.Ins.GetTemplateString("config/python3_json/include"));
            }

            GenerateCodeMonolithic(ctx,
                System.IO.Path.Combine(ctx.GenArgs.OutputCodeDir, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "Types.py")),
                lines,
                PreContent,
                null);
        }
    }
}
