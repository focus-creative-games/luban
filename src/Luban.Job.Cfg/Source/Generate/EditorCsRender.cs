using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Generate
{
    class EditorCsRender : CsCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/cs_editor_json/bean"));
            var result = template.Render(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/cs_editor_json/table"));
            var result = template.Render(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_stubRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/cs_editor_json/tables"));
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
