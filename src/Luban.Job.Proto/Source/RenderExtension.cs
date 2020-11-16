using Luban.Job.Proto.Defs;
using Scriban;
using System.Collections.Generic;

namespace Luban.Job.Proto
{
    public static class RenderExtension
    {
        public static string RenderCode(this Template template, object model, Dictionary<string, object> extraModels = null)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateExtends
            {
                ["x"] = model
            };
            if (extraModels != null)
            {
                foreach ((var k, var v) in extraModels)
                {
                    env[k] = v;
                }
            }
            ctx.PushGlobal(env);
            return template.Render(ctx);
        }
    }
}
