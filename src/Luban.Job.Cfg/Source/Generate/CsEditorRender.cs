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
    class CsEditorRender : CsCodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            var render = new CsEditorRender();
            foreach (var c in ctx.Assembly.Types.Values)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CS);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ELanguage.CS);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/cs_editor_json/bean");
            var result = template.Render(b);

            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/cs_editor_json/table");
            var result = template.Render(p);

            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/cs_editor_json/tables");
            var result = template.Render(new {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
