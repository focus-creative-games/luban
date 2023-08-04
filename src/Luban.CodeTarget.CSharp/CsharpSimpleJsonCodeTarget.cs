using Luban.CodeTarget.CSharp.TemplateExtensions;
using Scriban;

namespace Luban.CodeTarget.CSharp;

[CodeTarget("cs-simple-json")]
public class CsharpSimpleJsonCodeTarget : CsharpCodeTargetBase
{

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpSimpleJsonTemplateExtension());
    }
}