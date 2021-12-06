using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class ProtobufSchemaRenderBase : TemplateCodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            DefAssembly.LocalAssebmly.CurrentLanguage = Common.ELanguage.PROTOBUF;
            var file = RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "schema.proto");
            var content = this.RenderAll(ctx.ExportTypes);
            var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
            ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        }
    }
}
