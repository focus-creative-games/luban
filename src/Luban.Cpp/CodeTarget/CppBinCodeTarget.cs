using Luban.CodeTarget;
using Luban.Cpp.TemplateExtensions;
using Scriban;

namespace Luban.Cpp.CodeTarget;

[CodeTarget("cpp-bin")]
public class CppBinCodeTarget : CppCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CppBinTemplateExtension());
    }
}