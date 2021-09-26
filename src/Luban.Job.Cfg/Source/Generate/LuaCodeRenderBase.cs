using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class LuaCodeRenderBase : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            var file = "Types.lua";
            var content = this.RenderAll(ctx.ExportTypes);
            var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
            ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        }

        public override string Render(DefConst c)
        {
            throw new System.NotImplementedException();
        }

        public override string Render(DefEnum e)
        {
            throw new System.NotImplementedException();
        }

        public override string Render(DefTable c)
        {
            throw new System.NotImplementedException();
        }

        public override string Render(DefBean b)
        {
            throw new NotImplementedException();
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            throw new System.NotImplementedException();
        }

        public abstract string RenderAll(List<DefTypeBase> types);
    }
}
