using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_go_bin")]
    class GoCodeBinRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "go_bin";
    }
}
