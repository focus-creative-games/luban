using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class EditorCppRender
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

        public string Render(DefConst c)
        {
            return "// const";
        }

        public string Render(DefEnum e)
        {
            return "// enum";
        }

        public string Render(DefBean b)
        {
            return "// bean";
        }

        public string Render(DefTable p)
        {
            return "// table";
        }

        public string RenderStubs(string name, string module, List<CfgDefTypeBase> protos)
        {
            return "// stubs";
        }
    }
}
