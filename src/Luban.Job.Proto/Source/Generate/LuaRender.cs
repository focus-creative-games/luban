using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Proto.Generate
{
    class LuaRender
    {
        public string RenderTypes(List<DefTypeBase> types)
        {
            var consts = types.Where(t => t is DefConst).ToList();
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).ToList();
            var protos = types.Where(t => t is DefProto).ToList();
            var rpcs = types.Where(t => t is DefRpc).ToList();
            var template = StringTemplateUtil.GetTemplate("proto/lua/all");
            return template.RenderCode(new { Consts = consts, Enums = enums, Beans = beans, Protos = protos, Rpcs = rpcs });
        }
    }
}
