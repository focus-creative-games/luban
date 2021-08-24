using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    class TypescriptRender
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

                default: throw new Exception($"unknown render type:{o}");
            }
        }

        private string Render(DefConst c)
        {
            return RenderUtil.RenderTypescriptConstClass(c);
        }

        private string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_beanRender;
        private string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/typescript/bean"));
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_protoRender;
        private string Render(DefProto p)
        {
            var template = t_protoRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/typescript/proto"));
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_rpcRender;
        private string Render(DefRpc r)
        {
            var template = t_rpcRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/typescript/rpc"));
            var result = template.RenderCode(r);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStubs(string name, string module, List<DefTypeBase> protos, List<DefTypeBase> rpcs)
        {
            var template = t_stubRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("proto/typescript/stub"));
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
