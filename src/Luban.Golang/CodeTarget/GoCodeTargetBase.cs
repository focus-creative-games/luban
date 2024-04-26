using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Golang.TemplateExtensions;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

namespace Luban.Golang.CodeTarget;

public abstract class GoCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "go";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.GoDefaultCodeStyle;

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // go preserved key words 
        //"break", "default", "func", "interface", "select", "case", "defer", "go", "map", "struct", "chan", "else", "goto", "package", "switch", "const", "fallthrough", "if", "range", "continue", "for", "import", "return", "var"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new GoCommonTemplateExtension());
        string lubanModuleName = EnvManager.Current.GetOption(Name, "lubanGoModule", true);
        ctx.PushGlobal(new ScriptObject()
        {
            {"__luban_module_name", lubanModuleName},
        });
    }
}
