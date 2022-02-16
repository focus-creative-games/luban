using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cs_bin")]
    [Render("code_cs_unity_bin")]
    class CsCodeBinRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "cs_bin";
    }
}
