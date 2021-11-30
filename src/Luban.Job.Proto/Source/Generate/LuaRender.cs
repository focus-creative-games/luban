using Luban.Common.Protos;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    [Render("lua")]
    class LuaRender : RenderBase
    {
        public override void Render(GenContext ctx)
        {
            DefAssembly ass = ctx.Assembly;
            ctx.Tasks.Add(Task.Run(() =>
            {
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(RenderTypes(ctx.ExportTypes), Common.ELanguage.LUA);
                var file = "Types.lua";
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        public string RenderTypes(List<DefTypeBase> types)
        {
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).ToList();
            var protos = types.Where(t => t is DefProto).ToList();
            var rpcs = types.Where(t => t is DefRpc).ToList();
            var template = StringTemplateManager.Ins.GetTemplate("proto/lua/all");
            return template.RenderCode(new { Enums = enums, Beans = beans, Protos = protos, Rpcs = rpcs });
        }

        protected override string Render(DefEnum e)
        {
            throw new System.NotImplementedException();
        }

        protected override string Render(DefBean b)
        {
            throw new System.NotImplementedException();
        }

        protected override string Render(DefProto p)
        {
            throw new System.NotImplementedException();
        }

        protected override string Render(DefRpc r)
        {
            throw new System.NotImplementedException();
        }

        public override string RenderStubs(string name, string module, List<DefProto> protos, List<DefRpc> rpcs)
        {
            throw new System.NotImplementedException();
        }
    }
}
