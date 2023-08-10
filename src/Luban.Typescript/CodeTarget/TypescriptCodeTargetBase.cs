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

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new TypescriptCommonTemplateExtension());
    }
}