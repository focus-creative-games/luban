using Luban.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Scriban;

namespace Luban.CSharp;

[CodeTarget("cs-bin")]
public class CsharpBinCodeTarget : CsharpCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpBinTemplateExtension());
    }
}