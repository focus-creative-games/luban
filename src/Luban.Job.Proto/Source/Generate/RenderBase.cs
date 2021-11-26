using Luban.Common.Protos;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    abstract class RenderBase : IRender
    {
        public virtual void Render(GenContext ctx)
        {
            GenerateCodeScatter(ctx);
        }

        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefProto p: return Render(p);
                case DefRpc r: return Render(r);
                default: throw new Exception($"unknown render type:'{o}'");
            }
        }

        protected void GenerateCodeScatter(GenContext ctx)
        {
            ELanguage lan = ctx.Lan;
            var render = ctx.Render;
            DefAssembly ass = ctx.Assembly;
            foreach (var c in ctx.ExportTypes)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), lan);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, lan);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
            ctx.Tasks.Add(Task.Run(() =>
            {
                var module = ass.TopModule;
                var name = "ProtocolStub";
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                    render.RenderStubs(name, module,
                   ctx.ExportTypes.Where(t => t is DefProto).Cast<DefProto>().ToList(),
                    ctx.ExportTypes.Where(t => t is DefRpc).Cast<DefRpc>().ToList()),
                    lan);
                var file = RenderFileUtil.GetDefTypePath(name, lan);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        protected abstract string Render(DefEnum e);

        protected abstract string Render(DefBean b);

        protected abstract string Render(DefProto p);

        protected abstract string Render(DefRpc r);

        public abstract string RenderStubs(string name, string module, List<DefProto> protos, List<DefRpc> rpcs);
    }
}
