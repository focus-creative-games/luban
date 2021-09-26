using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_go_json")]
    class GoCodeJsonRender : GoCodeRenderBase
    {
        public override string Render(DefBean b)
        {
            string package = "cfg";

            var template = StringTemplateUtil.GetTemplate("config/go_json/bean");
            var result = template.RenderCode(b, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        public override string Render(DefTable p)
        {
            // TODO 目前只有普通表支持多态. 单例表和双key表都不支持
            string package = "cfg";
            var template = StringTemplateUtil.GetTemplate("config/go_json/table");
            var result = template.RenderCode(p, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            string package = "cfg";

            var template = StringTemplateUtil.GetTemplate("config/go_json/tables");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
                Package = package,
            });

            return result;
        }
    }
}
