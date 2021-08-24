using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class GoCodeBinRender : GoCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            string package = "cfg";

            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/go_bin/bean"));
            var result = template.RenderCode(b, new Dictionary<string, object>() { ["package"] = package });
            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            // TODO 目前只有普通表支持多态. 单例表和双key表都不支持
            string package = "cfg";
            var template = t_tableRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/go_bin/table"));
            var result = template.RenderCode(p, new Dictionary<string, object>() { ["package"] = package });
            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            string package = "cfg";

            var template = t_serviceRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/go_bin/tables"));
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
