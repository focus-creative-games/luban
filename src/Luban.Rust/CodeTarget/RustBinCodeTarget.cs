using Luban.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Luban.Rust.CodeTarget;
using Scriban;

namespace Luban.CSharp.CodeTarget;

[CodeTarget("rust-bin")]
public class RustBinCodeTarget : RustCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new RustBinTemplateExtension());
    }
}