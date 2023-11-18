using Luban.CodeTarget;
using Luban.Java.TemplateExtensions;
using Scriban;

namespace Luban.Java.CodeTarget;

[CodeTarget("java-json")]
public class JavaJsonCodeTarget : JavaCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new JavaJsonTemplateExtension());
    }
}
