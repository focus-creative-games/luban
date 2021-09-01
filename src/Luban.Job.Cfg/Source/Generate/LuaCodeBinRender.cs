using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_lua_bin")]
    class LuaCodeBinRender : LuaCodeRenderBase
    {
        public override string RenderAll(List<DefTypeBase> types)
        {
            var consts = types.Where(t => t is DefConst).ToList();
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).ToList();
            var tables = types.Where(t => t is DefTable).ToList();
            var template = StringTemplateUtil.GetOrAddTemplate("common/lua/base_all", fn =>
            Template.Parse(StringTemplateUtil.GetTemplateString("common/lua/base")
            + StringTemplateUtil.GetTemplateString("config/lua_bin/all")));
            return template.RenderCode(new { Consts = consts, Enums = enums, Beans = beans, Tables = tables });
        }
    }
}
