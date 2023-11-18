using Luban.CodeTarget;
using Luban.Gdscript.TemplateExtensions;
using Scriban;

namespace Luban.Gdscript.CodeTarget;

[CodeTarget("gdscript-json")]
public class GdscriptJsonCodeTarget : GdscriptCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new GdscriptJsonTemplateExtension());
    }
}
