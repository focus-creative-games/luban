using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Db.Generate
{
    class SyncCsRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable p: return Render(p);
                default: throw new Exception($"unknown render type:{o}");
            }
        }

        public string Render(DefConst c)
        {
            return RenderUtil.RenderCsConstClass(c);
        }

        public string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("db/cs_sync/bean"));
            var result = template.RenderCode(b);
            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("db/cs_sync/table"));
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderTables(string name, string module, List<DefTable> tables)
        {
            var template = t_stubRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("db/cs_sync/tables"));
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
