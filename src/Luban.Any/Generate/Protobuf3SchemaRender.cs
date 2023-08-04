namespace Luban.Any.Generate;

[Render("code_protobuf3")]
class Protobuf3SchemaRender : ProtobufSchemaRenderBase
{
    protected override string CommonRenderTemplateDir => "protobuf3";

    protected override string RenderTemplateDir => "protobuf3";
}