using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Typescript.TemplateExtensions;
using Scriban;

namespace Luban.Typescript.CodeTarget;

public abstract class TypescriptCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "ts";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.TypescriptDefaultCodeStyle;

    protected override string DefaultOutputFileName => "schema.ts";

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // typescript preserved key words
        // remove `type` because it's used frequently
        "abstract", "as", "any", "boolean", "break", "case", "catch", "class", "const", "continue", "debugger", "declare",
        "default", "delete", "do", "else", "enum", "export", "extends", "false", "finally", "for", "from", "function", "get",
        "if", "implements", "import", "in", "instanceof", "interface", "let", "module", "namespace", "new", "null", "number",
        "object", "package", "private", "protected", "public", "require", "return", "set", "static", "string", "super", "switch",
        "symbol", "this", "throw", "true", "try", /*"type",*/ "typeof", "undefined", "var", "void", "while", "with", "yield"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new TypescriptCommonTemplateExtension());
    }
}
