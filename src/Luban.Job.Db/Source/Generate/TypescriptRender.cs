using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
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

        private string Render(DefBean b)
        {
            return $"// {b.FullName}";
        }

        public string Render(DefTable c)
        {
            return $"// {c.FullName}";
        }

        public string RenderService(string name, string module, List<DefTable> tables)
        {

            return "// services";
        }
    }
}
