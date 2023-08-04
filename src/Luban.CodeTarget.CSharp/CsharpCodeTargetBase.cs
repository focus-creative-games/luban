using Luban.CodeTarget.CSharp.TemplateExtensions;
using Luban.Core.CodeFormat;
using Luban.Core.CodeTarget;
using Scriban;

namespace Luban.CodeTarget.CSharp;

public abstract class CsharpCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "cs";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.CsharpDefaultCodeStyle;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new CsharpTemplateExtension());
    }
}