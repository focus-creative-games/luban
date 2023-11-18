using Luban.CodeTarget;
using Luban.Golang.TemplateExtensions;
using Scriban;

namespace Luban.Golang.CodeTarget;

[CodeTarget("go-json")]
public class GoJsonCodeTarget : GoCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new GoJsonTemplateExtension());
    }
}
