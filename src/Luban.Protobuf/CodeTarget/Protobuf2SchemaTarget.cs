using Luban.CodeTarget;
using Luban.Protobuf.TemplateExtensions;
using Scriban;

namespace Luban.Protobuf.CodeTarget;

[CodeTarget("protobuf2")]
public class Protobuf2SchemaTarget : ProtobufSchemaTargetBase
{
    protected override string Syntax => "proto2";

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new Protobuf2TemplateExtension());
    }
}
