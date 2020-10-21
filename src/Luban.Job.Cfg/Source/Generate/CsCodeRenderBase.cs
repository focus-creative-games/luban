using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;

namespace Luban.Job.Cfg.Generate
{
    public abstract class CsCodeRenderBase : CodeRenderBase
    {
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
