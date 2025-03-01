using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Dart.TemplateExtensions;
using Scriban;

namespace Luban.Dart.CodeTarget;

public abstract class DartCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // dart preserved key words
        "if","else","for","while","do","switch","case","break","continue","return","var",
        "final","const","typedef","operator","external","async","await","sync*","async*",
        "class","extends","with","implements","abstract","base","interface","factory","get",
        "set","static","covariant","late","required","assert","yield","deferred","library",
        "part","import","export","as","on","dynamic","void","int","double",/*"num",*/"String",
        "bool","List","Map","Set","Runes","Symbol","@deprecated","@override","@protected",
        "@sealed","is","is!"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override string FileSuffixName => "dart";

    protected override string GetFileNameWithoutExtByTypeName(string name)
    {
        return name.Replace('.', '/');
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new DartCommonTemplateExtension());
    }

    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.DartDefaultCodeStyle;
}
