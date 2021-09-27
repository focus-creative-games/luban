using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class GoCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
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
