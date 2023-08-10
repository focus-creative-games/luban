using Luban.CodeTarget;
using Luban.Typescript.TemplateExtensions;
using Scriban;

namespace Luban.Typescript.CodeTarget;

[CodeTarget("ts-json")]
public class TypescriptJsonCodeTarget : TypescriptCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new TypescriptJsonTemplateExtension());
    }
}