namespace Luban.Any.Generate;

[Render("code_protobuf")]
[Render("code_protobuf2")]
class Protobuf2SchemaRender : ProtobufSchemaRenderBase
{
    protected override string CommonRenderTemplateDir => "protobuf2";

    protected override string RenderTemplateDir => "protobuf2";
}