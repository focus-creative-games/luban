using Luban.CodeTarget;
using Luban.Cpp.TemplateExtensions;
using Scriban;

namespace Luban.Cpp.CodeTarget;

[CodeTarget("cpp-sharedptr-bin")]
public class CppBinSharedptrCodeTarget : CppCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CppSharedptrBinTemplateExtension());
    }
}
