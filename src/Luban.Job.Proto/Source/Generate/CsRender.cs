using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Proto.Generate
{
    [Render("cs")]
    class CsRender : RenderBase
    {
        protected override string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }

        protected override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/bean");
            var result = template.RenderCode(b);

            return result;
        }

        protected override string Render(DefProto p)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/proto");
            var result = template.RenderCode(p);

            return result;
        }

        protected override string Render(DefRpc r)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/rpc");
            var result = template.RenderCode(r);

            return result;
        }

        public override string RenderStubs(string name, string module, List<DefProto> protos, List<DefRpc> rpcs)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/stub");
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
