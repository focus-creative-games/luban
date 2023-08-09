using Luban.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Scriban;

namespace Luban.CSharp;

[CodeTarget("cs-simple-json")]
public class CsharpSimpleJsonCodeTarget : CsharpCodeTargetBase
{

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpSimpleJsonTemplateExtension());
    }
}