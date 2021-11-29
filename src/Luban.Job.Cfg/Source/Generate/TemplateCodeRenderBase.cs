using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class TemplateCodeRenderBase : CodeRenderBase
    {
        protected abstract string CommonRenderTemplateDir { get; }

        protected abstract string RenderTemplateDir { get; }

        public override void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
        }

        public override string Render(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate($"common/{CommonRenderTemplateDir}/enum");
            var result = template.RenderCode(e);

            return result;
        }

        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate($"config/{RenderTemplateDir}/bean");
            var result = template.RenderCode(b);
            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate($"config/{RenderTemplateDir}/table");
            var result = template.RenderCode(p);
            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate($"config/{RenderTemplateDir}/tables");
            var result = template.RenderCode(new {
                Name = name,
                Namespace = module,
                Tables = tables,
            });
            return result;
        }

        public virtual string RenderAll(List<DefTypeBase> types)
        {
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).ToList();
            var tables = types.Where(t => t is DefTable).ToList();

            var template = StringTemplateUtil.GetTemplate($"config/{RenderTemplateDir}/all");
            var result = template.RenderCode(new {
                Namespace = DefAssembly.LocalAssebmly.TopModule,
                Enums = enums.Select(e => Render((DefEnum)e)).ToList(),
                Beans = beans.Select(b => Render((DefBean)b)).ToList(),
                Tables = tables.Select(t => Render((DefTable)t)).ToList(),
            });
            return result;
        }
    }
}
