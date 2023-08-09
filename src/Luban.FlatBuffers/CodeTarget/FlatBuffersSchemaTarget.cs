using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.FlatBuffers.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.FlatBuffers.CodeTarget;

[CodeTarget("flatbuffers")]
public class FlatBuffersSchemaTarget : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override string FileSuffixName => "fbs";
    
    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.NoneCodeStyle;

    protected override string DefaultOutputFileName => "schema.fbs";

    protected override string TemplateDir => "fbs";

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        
    }
}