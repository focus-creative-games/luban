using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_go_bin")]
    class GoCodeBinRender : GoCodeRenderBase
    {
        public override string Render(DefBean b)
        {
            string package = DefAssembly.LocalAssebmly.TopModule;

            var template = StringTemplateUtil.GetTemplate("config/go_bin/bean");
            var result = template.RenderCode(b, new Dictionary<string, object>() { ["package"] = package });
            return result;
        }

        public override string Render(DefTable p)
        {
            string package = DefAssembly.LocalAssebmly.TopModule;
            var template = StringTemplateUtil.GetTemplate("config/go_bin/table");
            var result = template.RenderCode(p, new Dictionary<string, object>() { ["package"] = package });
            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            string package = DefAssembly.LocalAssebmly.TopModule;
            var template = StringTemplateUtil.GetTemplate("config/go_bin/tables");
            var result = template.Render(new {
                Name = name,
                Namespace = module,
                Tables = tables,
                Package = package,
            });
            return result;
        }
    }
}
