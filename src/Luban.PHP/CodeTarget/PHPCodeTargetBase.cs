using Luban.CodeFormat;
using Luban.CodeFormat.CodeStyles;
using Luban.CodeTarget;
using Luban.PHP.TemplateExtensions;
using Scriban;

namespace Luban.PHP.CodeTarget;

public abstract class PHPCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_PHP;

    protected override string FileSuffixName => "php";

    private readonly static ICodeStyle s_codeStyle = new ConfigurableCodeStyle("pascal", "pascal", "camel", "camel", "camel", "none");

    protected override ICodeStyle DefaultCodeStyle => s_codeStyle;

    protected override string DefaultOutputFileName => "schema.php";

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // PHP preserved key words
        "__halt_compiler","abstract","and","array","as","break","callable","case","catch","class","clone","const","continue","declare",
        "default","die","do","echo","else","elseif","empty","enddeclare","endfor","endforeach","endif","endswitch","endwhile","eval",
        "exit","extends","final","finally","for","foreach","function","global","goto","if","implements","include","include_once","instanceof","insteadof","interface"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new PHPCommonTemplateExtension());
    }
}
