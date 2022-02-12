using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cs_unity_editor_json")]
    class CsUnityEditorRender : TemplateEditorJsonCodeRenderBase
    {
        override protected string RenderTemplateDir => "cs_unity_editor_json";
    }
}
