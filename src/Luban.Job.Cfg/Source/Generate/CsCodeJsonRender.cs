using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cs_json")]
    class CsCodeJsonRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "cs_json";
    }
}
