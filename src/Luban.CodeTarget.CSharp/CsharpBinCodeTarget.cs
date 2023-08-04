using Luban.CodeTarget.CSharp.TemplateExtensions;
using Scriban;

namespace Luban.CodeTarget.CSharp;

[CodeTarget("cs-bin")]
public class CsharpBinCodeTarget : CsharpCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpBinTemplateExtension());
    }
}