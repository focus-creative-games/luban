using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_java_json")]
    class JavaCodeJsonRender : JavaCodeRenderBase
    {
        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/java_json/bean");
            var result = template.RenderCode(b);

            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/java_json/table");
            var result = template.RenderCode(p);

            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/java_json/tables");
            var result = template.Render(new {
                Name = name,
                Package = module,
                Tables = tables,
            });

            return result;
        }
    }
}
