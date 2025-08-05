using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Javascript.TemplateExtensions;
using Scriban;

namespace Luban.Javascript.CodeTarget;

public abstract class JavascriptCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "js";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.TypescriptDefaultCodeStyle;

    protected override string DefaultOutputFileName => "schema.js";

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // typescript preserved key words
        // remove `type` because it's used frequently
       "class", "function", "var", "let", "const", "if", "else", "switch", "case", "default",
        "for", "while", "do", "break", "continue", "return", "try", "catch", "finally", "throw",
        "new", "delete", "typeof", "instanceof", "void", "this", "super", "extends", "import",
        "export", "from", "as", "in", "of", "await", "async", "yield", "with", "debugger",
        "static", "get", "set",

        "enum", "implements", "interface", "package", "private", "protected", "public"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new JavascriptCommonTemplateExtension());
    }
}
