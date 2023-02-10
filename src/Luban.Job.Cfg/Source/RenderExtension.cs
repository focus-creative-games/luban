using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Utils;
using Scriban;
using System.Collections.Generic;

namespace Luban.Job.Cfg
{
    public static class RenderExtension
    {
        public static string RenderCode(this Template template, object model, Dictionary<string, object> extraModels = null)
        {
            var ctx = TemplateUtil.CreateDefaultTemplateContext();
            var env = new TTypeTemplateExtends
            {
                ["x"] = model,
                ["assembly"] = DefAssembly.LocalAssebmly,
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

        public static string RenderDatas(this Template template, DefTable table, List<DBean> exportDatas, Dictionary<string, object> extraModels = null)
        {
            var ctx = TemplateUtil.CreateDefaultTemplateContext();
            var env = new DTypeTemplateExtends
            {
                ["table"] = table,
                ["datas"] = exportDatas,
                ["assembly"] = DefAssembly.LocalAssebmly,
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


        public static string RenderData(this Template template, DefTable table, DBean data, Dictionary<string, object> extraModels = null)
        {
            var ctx = TemplateUtil.CreateDefaultTemplateContext();
            var env = new DTypeTemplateExtends
            {
                ["table"] = table,
                ["data"] = data,
                ["assembly"] = DefAssembly.LocalAssebmly,
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
