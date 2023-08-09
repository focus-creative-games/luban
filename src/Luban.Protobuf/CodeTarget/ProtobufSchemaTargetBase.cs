using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Protobuf.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.Protobuf.CodeTarget;

public abstract class ProtobufSchemaTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override string FileSuffixName => "pb";
    
    protected abstract string Syntax { get; }

    protected override string TemplateDir => "pb";

    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.NoneCodeStyle;

    protected override string DefaultOutputFileName => "schema.proto";

    protected override string GenerateSchema(GenerationContext ctx)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"schema");
        var tplCtx = CreateTemplateContext(template);
        tplCtx.PushGlobal(new ProtobufCommonTemplateExtension());
        OnCreateTemplateContext(tplCtx);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager},
            { "__namespace", ctx.Target.TopModule},
            { "__tables", ctx.ExportTables},
            { "__beans", ctx.ExportBeans},
            { "__enums", ctx.ExportEnums},
            { "__code_style", CodeStyle},
            { "__syntax", Syntax},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        return writer.ToResult(FileHeader);
    }
}