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

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new PythonCommonTemplateExtension());
    }
}