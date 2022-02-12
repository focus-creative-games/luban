using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cpp_ue_editor_json")]
    class CppUE4EditorJsonRender : TemplateEditorJsonCodeRenderBase
    {
        protected override string RenderTemplateDir => "cpp_ue_editor_json";

        public override void Render(GenContext ctx)
        {
            var render = new CppUE4EditorJsonRender();

            var renderTypes = ctx.Assembly.Types.Values.Where(c => c is DefEnum || c is DefBean).ToList();

            foreach (var c in renderTypes)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CPP);
                    var file = RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }

            int TYPE_PER_STUB_FILE = int.Parse(ctx.Assembly.GetOptionOr($"{RenderTemplateDir}.type_per_stub_file", "200"));
            string stubFileFormat = ctx.Assembly.GetOptionOr($"{RenderTemplateDir}.stub_file_name_format", "gen_stub_{0}.cpp");
            var template = GetConfigTemplate("stub");
            for (int i = 0, n = (renderTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
            {
                int index = i;
                ctx.Tasks.Add(Task.Run(() =>
                {
                    int startIndex = index * TYPE_PER_STUB_FILE;
                    var rawContent = template.RenderCode(new { Types = renderTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, renderTypes.Count - startIndex)), });
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(rawContent, ELanguage.CPP);
                    var file = string.Format(stubFileFormat, index);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }
    }
}
