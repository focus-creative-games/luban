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
