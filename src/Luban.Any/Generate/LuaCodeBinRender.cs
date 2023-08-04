namespace Luban.Any.Generate;

[Render("code_lua_bin")]
class LuaCodeBinRender : LuaCodeRenderBase
{
    protected override string RenderTemplateDir => "lua_bin";

    public override string RenderAll(List<DefTypeBase> types)
    {
        var enums = types.Where(t => t is DefEnum).ToList();
        var beans = types.Where(t => t is DefBean).ToList();
        var tables = types.Where(t => t is DefTable).ToList();
        var template = StringTemplateManager.Ins.GetOrAddTemplate("common/lua/base_all", fn =>
            Template.Parse(StringTemplateManager.Ins.GetTemplateString("common/lua/base")
                           + StringTemplateManager.Ins.GetTemplateString("config/lua_bin/all")));
        return template.RenderCode(new { Enums = enums, Beans = beans, Tables = tables });
    }
}