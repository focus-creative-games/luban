using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Proto.Generate
{
    class CsRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefProto p: return Render(p);
                case DefRpc r: return Render(r);

                default: throw new Exception($"unknown render type:'{o}'");
            }
        }

        private string Render(DefConst c)
        {
            return RenderUtil.RenderCsConstClass(c);
        }

        private string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }

        private string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/bean");
            var result = template.RenderCode(b);

            return result;
        }

        private string Render(DefProto p)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/proto");
            var result = template.RenderCode(p);

            return result;
        }

        private string Render(DefRpc r)
        {
            var template = StringTemplateUtil.GetTemplate("proto/cs/rpc");
            var result = template.RenderCode(r);

            return result;
        }

        public string RenderStubs(string name, string module, List<DefTypeBase> protos, List<DefTypeBase> rpcs)
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
