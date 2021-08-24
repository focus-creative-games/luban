using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class JavaCodeBinRender : JavaCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;

        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/java_bin/bean"));
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/java_bin/table"));
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/java_bin/tables"));
            var result = template.Render(new
            {
                Name = name,
                Package = module,
                Tables = tables,
            });

            return result;
        }
    }
}
