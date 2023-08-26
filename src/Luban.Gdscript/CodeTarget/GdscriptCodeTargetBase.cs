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

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new GdscriptCommonTemplateExtension());
    }
}