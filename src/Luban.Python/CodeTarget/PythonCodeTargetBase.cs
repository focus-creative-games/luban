using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Python.TemplateExtensions;
using Scriban;
using Scriban.Runtime;

namespace Luban.Python.CodeTarget;

public abstract class PythonCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_PYTHON;

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.PythonDefaultCodeStyle;

    protected override string FileSuffixName => "py";

    protected override string DefaultOutputFileName => "schema.py";

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // python preserved key words
        "False", "None", "True", "and", "as", "assert", "async", "await", "break", "class", "continue",
        "def", "del", "elif", "else", "except", "finally", "for", "from", "global", "if", "import", "in",
        "is", "lambda", "nonlocal", "not", "or", "pass", "raise", "return", "try", "while", "with", "yield"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new PythonCommonTemplateExtension());
    }
}
