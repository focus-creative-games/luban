using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;

namespace Luban.Job.Cfg.Generate
{
    class Python3CodeJsonRender : PythonCodeRenderBase
    {
        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/python_json/bean");
            var result = template.RenderCode(b);

            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/python_json/table");
            var result = template.RenderCode(p);

            return result;
        }
    }
}
