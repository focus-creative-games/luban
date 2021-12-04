using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_java_bin")]
    class JavaCodeBinRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "java_bin";
    }
}
