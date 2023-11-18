using Luban.CodeTarget;
using Luban.Lua.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.Lua.CodeTarget;

[CodeTarget("lua-bin")]
public class LuaBinCodeTarget : LuaCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new LuaBinTemplateExtension());
    }
}
