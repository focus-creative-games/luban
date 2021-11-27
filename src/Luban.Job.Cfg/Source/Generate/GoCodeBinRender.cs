using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_go_bin")]
    class GoCodeBinRender : GoCodeRenderBase
    {
        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/go_bin/bean");
            var result = template.RenderCode(b);
            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/go_bin/table");
            var result = template.RenderCode(p);
            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/go_bin/tables");
            var result = template.Render(new {
                Name = name,
                Namespace = module,
                Tables = tables,
            });
            return result;
        }
    }
}
