using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_typescript_bin")]
    class TypescriptCodeBinRender : TypescriptCodeRenderBase
    {
        protected override string RenderTemplateDir => "typescript_bin";
    }
}
