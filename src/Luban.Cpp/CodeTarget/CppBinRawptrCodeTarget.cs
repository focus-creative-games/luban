using Luban.CodeTarget;
using Luban.Cpp.TemplateExtensions;
using Scriban;

namespace Luban.Cpp.CodeTarget;

[CodeTarget("cpp-rawptr-bin")]
public class CppBinRawptrCodeTarget : CppCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CppRawptrBinTemplateExtension());
    }
}
