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

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // protobuf schema preserved key words
        "package", "optional", "import", "message", "enum", "service", "rpc", "stream", "returns", "oneof", "map", "reserved",
        "to", "true", "false", "syntax", "repeated", "required", "extend", "extensions", "group", "default", "packed", "option",
        "int32", "int64", "uint32", "uint64", "sint32", "sint64", "fixed32", "fixed64", "sfixed32", "sfixed64", "float", "double", "bool", "string", "bytes"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

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
