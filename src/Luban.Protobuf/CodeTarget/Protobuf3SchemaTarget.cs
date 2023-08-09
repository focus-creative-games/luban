using Luban.CodeTarget;
using Luban.Protobuf.TemplateExtensions;
using Scriban;

namespace Luban.Protobuf.CodeTarget;

[CodeTarget("pb3")]
public class Protobuf3SchemaTarget : ProtobufSchemaTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new Protobuf3TemplateExtension());
    }
}