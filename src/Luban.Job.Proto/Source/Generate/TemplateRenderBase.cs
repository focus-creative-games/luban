using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    abstract class TemplateRenderBase : RenderBase
    {
        protected string RenderTemplateDir => (this.GetType().GetCustomAttributes(typeof(RenderAttribute), false)[0] as RenderAttribute).Name;

        protected override string Render(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate($"common/{RenderTemplateDir}/enum");
            var result = template.Render(e);
            return result;
        }

        protected override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate($"proto/{RenderTemplateDir}/bean");
            var result = template.RenderCode(b);
            return result;
        }

        protected override string Render(DefProto p)
        {
            var template = StringTemplateUtil.GetTemplate($"proto/{RenderTemplateDir}/proto");
            var result = template.RenderCode(p);
            return result;
        }

        protected override string Render(DefRpc r)
        {
            var template = StringTemplateUtil.GetTemplate($"proto/{RenderTemplateDir}/rpc");
            var result = template.RenderCode(r);
            return result;
        }

        public override string RenderStubs(string name, string module, List<DefProto> protos, List<DefRpc> rpcs)
        {
            var template = StringTemplateUtil.GetTemplate($"proto/{RenderTemplateDir}/stub");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Protos = protos,
                Rpcs = rpcs,
            });

            return result;
        }
    }
}
