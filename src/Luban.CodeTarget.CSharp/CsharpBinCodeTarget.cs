using Luban.CodeTarget.CSharp.TemplateExtensions;
using Luban.Core.CodeTarget;
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