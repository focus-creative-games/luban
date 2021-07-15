using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class TypescriptCodeRenderBase : CodeRenderBase
    {
        public override string Render(DefConst c)
        {
            return RenderUtil.RenderTypescriptConstClass(c);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
        }
    }
}
