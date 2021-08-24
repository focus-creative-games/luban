using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;

namespace Luban.Job.Cfg.Generate
{
    class Python3CodeJsonRender : PythonCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/python_json/bean"));
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/python_json/table"));
            var result = template.RenderCode(p);

            return result;
        }
    }
}
