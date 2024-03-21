using Luban.CodeTarget;
using Luban.Lua.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.Lua.CodeTarget;

public abstract class LuaCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_LUA;

    protected override string FileSuffixName => "lua";

    protected override string DefaultOutputFileName => "schema.lua";


    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // lua preserved key words
        "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "goto", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new LuaCommonTemplateExtension());
    }
}
