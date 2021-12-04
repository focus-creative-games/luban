using Luban.Job.Common;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_template")]
    class TemplateCodeScatterRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => GenContext.Ctx.GenArgs.TemplateCodeDir;

        protected override ELanguage GetLanguage(GenContext ctx)
        {
            return RenderFileUtil.GetLanguage(ctx.GenArgs.TemplateCodeDir);
        }
    }
}
