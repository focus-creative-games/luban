using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class PythonCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            ctx.Render = this;
            ctx.Lan = Common.ELanguage.PYTHON;

            var lines = new List<string>(10000);
            static void PreContent(List<string> fileContent)
            {
                fileContent.Add(PythonStringTemplates.ImportTython3Enum);
                fileContent.Add(PythonStringTemplates.PythonVectorTypes);
            }

            GenerateCodeMonolithic(ctx, "Types.py", lines, PreContent, null);
        }

        public override string Render(DefConst c)
        {
            return RenderUtil.RenderPythonConstClass(c);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderPythonEnumClass(e);
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/python_json/tables");
            var result = template.RenderCode(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
