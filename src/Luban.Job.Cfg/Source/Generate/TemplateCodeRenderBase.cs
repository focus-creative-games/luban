using Luban.Job.Cfg.Defs;
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
    abstract class TemplateCodeRenderBase : CodeRenderBase
    {
        protected virtual string CommonRenderTemplateDir => RenderFileUtil.GetCommonTemplateDirName(DefAssembly.LocalAssebmly.CurrentLanguage);

        protected abstract string RenderTemplateDir { get; }

        protected Scriban.Template GetConfigTemplate(string name)
        {
            return StringTemplateManager.Ins.GetTemplate($"config/{RenderTemplateDir}/{name}");
        }

        protected Scriban.Template GetCommonTemplate(string name)
        {
            return StringTemplateManager.Ins.GetTemplate($"common/{CommonRenderTemplateDir}/{name}");
        }

        public override void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
        }

        public override string Render(DefEnum e)
        {
            var template = GetCommonTemplate("enum");
            var result = template.RenderCode(e);

            return result;
        }

        public override string Render(DefBean b)
        {
            var template = GetConfigTemplate("bean");
            var result = template.RenderCode(b);
            return result;
        }

        public override string Render(DefTable p)
        {
            var template = GetConfigTemplate("table");
            var result = template.RenderCode(p);
            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = GetConfigTemplate("tables");
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

            var template = GetConfigTemplate("all");
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
