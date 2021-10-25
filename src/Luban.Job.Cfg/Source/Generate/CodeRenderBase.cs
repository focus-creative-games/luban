using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class CodeRenderBase : ICfgCodeRender
    {
        public abstract void Render(GenContext ctx);


        public abstract string Render(DefEnum c);
        public abstract string Render(DefBean b);
        public abstract string Render(DefTable c);
        public abstract string RenderService(string name, string module, List<DefTable> tables);

        public string RenderAny(DefTypeBase o)
        {
            switch (o)
            {
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable r: return Render(r);
                default: throw new Exception($"unknown render type:'{o}'");
            }
        }

        protected void GenerateCodeScatter(GenContext ctx)
        {
            string genType = ctx.GenType;
            ctx.Render = this;
            ctx.Lan = RenderFileUtil.GetLanguage(genType);
            DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Lan;
            foreach (var c in ctx.ExportTypes)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(ctx.Render.RenderAny(c), ctx.Lan);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ctx.Lan);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }

            ctx.Tasks.Add(Task.Run(() =>
            {
                var module = ctx.TopModule;
                var name = ctx.TargetService.Manager;
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(ctx.Render.RenderService(name, module, ctx.ExportTables), ctx.Lan);
                var file = RenderFileUtil.GetDefTypePath(name, ctx.Lan);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        protected void GenerateCodeMonolithic(GenContext ctx, string outputFile, List<string> fileContent, Action<List<string>> preContent, Action<List<string>> postContent)
        {
            ctx.Tasks.Add(Task.Run(() =>
            {
                fileContent.Add(FileHeaderUtil.GetAutoGenerationHeader(ctx.Lan));

                preContent?.Invoke(fileContent);

                foreach (var type in ctx.ExportTypes)
                {
                    fileContent.Add(ctx.Render.RenderAny(type));
                }

                fileContent.Add(ctx.Render.RenderService("Tables", ctx.TopModule, ctx.ExportTables));
                postContent?.Invoke(fileContent);

                var file = outputFile;
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join("\n", fileContent));
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

    }
}
