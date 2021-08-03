using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    public abstract class CodeRenderBase : ICfgCodeRender
    {
        public abstract string Render(DefConst c);
        public abstract string Render(DefEnum c);
        public abstract string Render(DefBean b);
        public abstract string Render(DefTable c);
        public abstract string RenderService(string name, string module, List<DefTable> tables);

        public string RenderAny(DefTypeBase o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable r: return Render(r);
                default: throw new Exception($"unknown render type:'{o}'");
            }
        }
    }
}
