using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_rust_json")]
    class RustCodeJsonRender : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            string genType = ctx.GenType;
            var args = ctx.GenArgs;
            ctx.Render = this;
            ctx.Lan = RenderFileUtil.GetLanguage(genType);
            DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Lan;

            var lines = new List<string>();
            GenerateCodeMonolithic(ctx, RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "mod.rs"), lines, ls =>
            {
                var template = StringTemplateUtil.GetTemplate("config/rust_json/include");
                var result = template.RenderCode(ctx.ExportTypes);
                ls.Add(result);
            }, null);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderRustEnumClass(e);
        }

        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/rust_json/bean");
            var result = template.RenderCode(b);

            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/rust_json/table");
            var result = template.RenderCode(p);

            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/rust_json/tables");
            var result = template.RenderCode(new {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
