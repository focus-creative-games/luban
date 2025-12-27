using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Kotlin.TemplateExtensions;
using Luban.Utils;
using Scriban;

namespace Luban.Kotlin.CodeTarget;

public abstract class KotlinCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "kt";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.JavaDefaultCodeStyle;

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // kotlin preserved key words
        "abstract", "actual", "annotation", "as", "break", "by", "catch", "class", "companion", "const", "constructor",
        "continue", "crossinline", "delegate", "do", "dynamic", "else", "enum", "expect", "external", "false",
        "field", "file", "final", "finally", "for", "fun", "get", "if", "import", "in", "infix", "init", "inline",
        "inner", "interface", "internal", "is", "it", "lateinit", "noinline", "null", "object", "open", "operator",
        "out", "override", "package", "param", "private", "property", "protected", "public", "receiver", "reified",
        "return", "sealed", "set", "setparam", "super", "suspend", "tailrec", "this", "throw", "true", "try", "typealias",
        "typeof", "val", "var", "vararg", "when", "where", "while"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override string GetFileNameWithoutExtByTypeName(string name)
    {
        return name.Replace('.', '/');
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new KotlinCommonTemplateExtension());
    }
}
