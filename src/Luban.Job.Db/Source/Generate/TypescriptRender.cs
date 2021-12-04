using Luban.Job.Common.Defs;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Db.Generate
{
    class TypescriptRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable r: return Render(r);
                default: throw new Exception($"unknown render type:{o}");
            }
        }

        private string Render(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/typescript/enum");
            var result = template.Render(e);
            return result;
        }

        public string Render(DefBean b)
        {
            var template = StringTemplateManager.Ins.GetTemplate("db/typescript/bean");
            var result = template.RenderCode(b);
            return result;
        }

        public string Render(DefTable p)
        {
            var template = StringTemplateManager.Ins.GetTemplate("db/typescript/table");
            var result = template.RenderCode(p);

            return result;
        }

        public string RenderTables(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateManager.Ins.GetTemplate("db/typescript/tables");
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
