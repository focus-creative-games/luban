using Luban.CodeTarget;
using Luban.Javascript.TemplateExtensions;
using Scriban;

namespace Luban.Javascript.CodeTarget;

[CodeTarget("javascript-bin")]
public class TypescriptBinCodeTarget : JavascriptCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new JavascriptBinTemplateExtension());
    }
}
