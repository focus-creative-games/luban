using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Tpl;
using Luban.Job.Common.TypeVisitors;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_flatbuffers")]
    class FlatBuffersSchemaRender : TemplateCodeRenderBase
    {
        protected override string RenderTemplateDir => "flatbuffers";

        public override void Render(GenContext ctx)
        {
            {
                DefAssembly.LocalAssebmly.CurrentLanguage = Common.ELanguage.FLATBUFFERS;
                var file = RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "schema.fbs");
                var content = this.RenderAll(ctx.ExportTypes);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }

            {
                DefAssembly.LocalAssebmly.CurrentLanguage = Common.ELanguage.FLATBUFFERS;
                var file = "convert_json_to_binary.bat";
                var content = this.RenderConvertJson2BinaryBat(ctx.Assembly.TableManagerName, ctx.TopModule, ctx.ExportTables);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }
            {
                DefAssembly.LocalAssebmly.CurrentLanguage = Common.ELanguage.FLATBUFFERS;
                var file = "convert_json_to_binary.sh";
                var content = this.RenderConvertJson2BinarySh(ctx.Assembly.TableManagerName, ctx.TopModule, ctx.ExportTables);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }
        }

        private string RenderConvertJson2BinaryBat(string name, string module, List<DefTable> tables)
        {
            var template = GetConfigTemplate("convert_json_to_binary_bat");
            var result = template.RenderCode(new {
                Name = name,
                Namespace = module,
                Tables = tables,
            });
            return result;
        }

        private string RenderConvertJson2BinarySh(string name, string module, List<DefTable> tables)
        {
            var template = GetConfigTemplate("convert_json_to_binary_sh");
            var result = template.RenderCode(new {
                Name = name,
                Namespace = module,
                Tables = tables,
            });
            return result;
        }

        private MapKeyValueEntryCollection CollectKeyValueEntry(List<DefBean> beans)
        {
            var c = new MapKeyValueEntryCollection();

            foreach (DefBean bean in beans)
            {
                CollectMapKeyValueEntrysVisitor.Ins.Accept(bean, c);
            }

            return c;
        }

        public override string RenderAll(List<DefTypeBase> types)
        {
            var ass = DefAssembly.LocalAssebmly;
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).Cast<DefBean>().ToList();
            var tables = types.Where(t => t is DefTable).ToList();

            var maps = CollectKeyValueEntry(beans).KeyValueEntries.Values;

            // 多态在flatbuffers中为union类型。
            // flatbuffers要求union必须在使用前定义
            // 所以排到前面生成
            beans.Sort((a, b) => (a.IsAbstractType ? 0 : 1) - (b.IsAbstractType ? 0 : 1));

            var template = GetConfigTemplate("all");
            var result = template.RenderCode(new {
                Namespace = ass.TopModule,
                Enums = enums.Select(e => Render((DefEnum)e)).ToList(),
                Beans = beans.Select(b => Render(b)).ToList(),
                Tables = tables.Select(t => Render((DefTable)t)).ToList(),
                Maps = maps,
            });
            return result;
        }
    }
}
