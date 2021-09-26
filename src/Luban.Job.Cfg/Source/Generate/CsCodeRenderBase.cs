using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;

namespace Luban.Job.Cfg.Generate
{
    abstract class CsCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
        }

        public override string Render(DefConst c)
        {
            return RenderUtil.RenderCsConstClass(c);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }
    }
}
