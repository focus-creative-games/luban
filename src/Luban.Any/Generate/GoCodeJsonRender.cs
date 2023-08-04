namespace Luban.Any.Generate;

[Render("code_go_json")]
class GoCodeJsonRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => "go_json";
}