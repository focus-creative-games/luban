using Luban.CodeTarget;
using Luban.Dart.TemplateExtensions;
using Scriban;

namespace Luban.Dart.CodeTarget;

[CodeTarget("dart-json")]
class DartJsonCodeTarget : DartCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new DartJsonTemplateExtension());
    }
}
