using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_gdscript_json")]
    internal class GDScriptCodeJsonRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "gdscript_json";

        public override void Render(GenContext ctx)
        {
            ctx.Render = this;
            ctx.Lan = Common.ELanguage.GDSCRIPT;
            DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Lan;

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
}
