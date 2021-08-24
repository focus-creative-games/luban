using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using Scriban;
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

        [ThreadStatic]
        private static Template t_beanRender;
        private string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/cs/bean"));
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_protoRender;
        private string Render(DefProto p)
        {
            var template = t_protoRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/cs/proto"));
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_rpcRender;
        private string Render(DefRpc r)
        {
            var template = t_rpcRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/cs/rpc"));
            var result = template.RenderCode(r);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStubs(string name, string module, List<DefTypeBase> protos, List<DefTypeBase> rpcs)
        {
            var template = t_stubRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/cs/stub"));
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
