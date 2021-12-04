using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_go_json")]
    class GoCodeJsonRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "go_json";
    }
}
