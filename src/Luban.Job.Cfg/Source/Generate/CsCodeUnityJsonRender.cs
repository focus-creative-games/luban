using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cs_unity_json")]
    class CsCodeUnityJsonRender : CsCodeRenderBase
    {
        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/cs_unity_json/bean");
            var result = template.RenderCode(b);

            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/cs_unity_json/table");
            var result = template.RenderCode(p);

            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/cs_unity_json/tables");
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
