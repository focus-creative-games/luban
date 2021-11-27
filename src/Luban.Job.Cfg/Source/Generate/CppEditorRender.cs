using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cpp_editor")]
    class CppEditorRender : TemplateCodeRenderBase
    {
        protected override string CommonRenderTemplateDir => "cpp";

        protected override string RenderTemplateDir => "cpp_editor_json";
    }
}
