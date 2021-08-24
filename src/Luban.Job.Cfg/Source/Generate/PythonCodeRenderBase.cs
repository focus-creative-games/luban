using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class PythonCodeRenderBase : CodeRenderBase
    {
        public override string Render(DefConst c)
        {
            return RenderUtil.RenderPythonConstClass(c);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderPythonEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("config/python_json/tables"));
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
