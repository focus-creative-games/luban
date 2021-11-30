using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using Scriban;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Generate
{
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
}
