using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Gdscript.TemplateExtensions;
using Scriban;

namespace Luban.Gdscript.CodeTarget;

public abstract class GdscriptCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_GDSCRIPT;

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.PythonDefaultCodeStyle;

    protected override string FileSuffixName => "gd";

    protected override string DefaultOutputFileName => "schema.gd";


    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // gdscript preserved key words
        "and", "as", "assert", "break", "class", "const", "continue", "elif", "else", "enum", "extends", "for", "if", "in", "is", "pass", "return", "self", "static", "while"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new GdscriptCommonTemplateExtension());
    }
}
