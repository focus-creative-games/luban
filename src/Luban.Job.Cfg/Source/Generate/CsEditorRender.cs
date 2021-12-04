using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cs_unity_editor")]
    class CsEditorRender : TemplateCodeRenderBase
    {
        override protected string RenderTemplateDir => "cs_editor_json";

        public override void Render(GenContext ctx)
        {
            foreach (var c in ctx.Assembly.Types.Values)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(RenderAny(c), ELanguage.CS);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ELanguage.CS);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }


    }
}
