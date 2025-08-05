using Luban.CodeTarget;
using Luban.Javascript.TemplateExtensions;
using Scriban;

namespace Luban.Javascript.CodeTarget;

[CodeTarget("javascript-json")]
public class JavascriptJsonCodeTarget : JavascriptCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new JavascriptJsonTemplateExtension());
    }
}
