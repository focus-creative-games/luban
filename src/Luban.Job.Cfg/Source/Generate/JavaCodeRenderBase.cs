using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class JavaCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
        }

        public override string Render(DefConst c)
        {
            return RenderUtil.RenderJavaConstClass(c);
        }

        public override string Render(DefEnum c)
        {
            return RenderUtil.RenderJavaEnumClass(c);
        }
    }
}
