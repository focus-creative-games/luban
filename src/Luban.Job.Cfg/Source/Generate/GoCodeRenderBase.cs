using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class GoCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
        }

        public override string Render(DefConst c)
        {
            string package = "cfg";
            var template = StringTemplateUtil.GetTemplate("common/go/const");
            var result = template.RenderCode(c, new Dictionary<string, object>() { ["package"] = package });
            return result;
        }

        public override string Render(DefEnum e)
        {
            string package = "cfg";
            var template = StringTemplateUtil.GetTemplate("common/go/enum");
            var result = template.RenderCode(e, new Dictionary<string, object>() { ["package"] = package });
            return result;
        }

    }
}
