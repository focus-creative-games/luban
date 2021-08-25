using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Db.Generate
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
                case DefTable r: return Render(r);
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

        public string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("db/typescript/bean");
            var result = template.RenderCode(b);
            return result;
        }

        public string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("db/typescript/table");
            var result = template.RenderCode(p);

            return result;
        }

        public string RenderTables(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("db/typescript/tables");
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
