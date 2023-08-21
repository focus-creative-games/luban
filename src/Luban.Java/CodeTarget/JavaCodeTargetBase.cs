using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Java.TemplateExtensions;
using Luban.Utils;
using Scriban;

namespace Luban.Java.CodeTarget;

public abstract class JavaCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "java";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.JavaDefaultCodeStyle;

    protected override string GetFileNameWithoutExtByTypeName(string name)
    {
        return name.Replace('.', '/');
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new JavaCommonTemplateExtension());
    }
}