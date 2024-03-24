using Luban.CodeTarget;
using Luban.PHP.TemplateExtensions;
using Scriban;

namespace Luban.PHP.CodeTarget;

[CodeTarget("php-json")]
public class PHPJsonCodeTarget : PHPCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new PHPJsonTemplateExtension());
    }
}
