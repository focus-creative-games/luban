using Luban.CodeTarget;
using Luban.Kotlin.TemplateExtensions;
using Scriban;

namespace Luban.Kotlin.CodeTarget;

[CodeTarget("kotlin-bin")]
public class KotlinBinCodeTarget : KotlinCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new KotlinBinTemplateExtension());
    }
}