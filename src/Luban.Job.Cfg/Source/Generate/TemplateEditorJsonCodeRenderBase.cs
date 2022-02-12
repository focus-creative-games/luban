using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class TemplateEditorJsonCodeRenderBase :TemplateCodeRenderBase
    {
        public override string Render(DefEnum e)
        {
            var template = GetConfigTemplate("enum");
            var result = template.RenderCode(e);
            return result;
        }

        public override void Render(GenContext ctx)
        {
            ELanguage lan = GetLanguage(ctx);
            ctx.Assembly.CurrentLanguage = lan;
            foreach (var c in ctx.Assembly.Types.Values)
            {
                if (c is not DefBean && c is not DefEnum)
                {
                    continue;
                }
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(RenderAny(c), lan);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, lan);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }
    }
}
