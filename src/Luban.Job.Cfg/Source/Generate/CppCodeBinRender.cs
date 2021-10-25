using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cpp_bin")]
    class CppCodeBinRender : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            // 将所有 头文件定义 生成到一个文件
            // 按照 const,enum,bean,table, service 的顺序生成
            DefAssembly.LocalAssebmly.CurrentLanguage = ELanguage.CPP;
            ctx.Tasks.Add(Task.Run(() =>
            {
                var headerFileContent = new List<string>
                                {
                                    @$"
#pragma once
#include <functional>

#include ""bright/serialization/ByteBuf.h""
#include ""bright/CfgBean.hpp""

using ByteBuf = ::bright::serialization::ByteBuf;

namespace {ctx.TopModule}
{{
"
                                };

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefEnum e)
                    {
                        headerFileContent.Add(Render(e));
                    }
                }

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefBean e)
                    {
                        headerFileContent.Add(RenderForwardDefine(e));
                    }
                }

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefBean e)
                    {
                        headerFileContent.Add(Render(e));
                    }
                }

                foreach (var type in ctx.ExportTables)
                {
                    headerFileContent.Add(Render(type));
                }

                headerFileContent.Add(RenderService("Tables", ctx.TopModule, ctx.ExportTables));

                headerFileContent.Add("}"); // end of topmodule

                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', headerFileContent), ELanguage.CPP);
                var file = "gen_types.h";
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));

            var beanTypes = ctx.ExportTypes.Where(c => c is DefBean).ToList();

            int TYPE_PER_STUB_FILE = 100;

            for (int i = 0, n = (beanTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
            {
                int index = i;
                ctx.Tasks.Add(Task.Run(() =>
                {
                    int startIndex = index * TYPE_PER_STUB_FILE;
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                        RenderStub(ctx.TopModule, beanTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, beanTypes.Count - startIndex))),
                        ELanguage.CPP);
                    var file = $"gen_stub_{index}.cpp";
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        public override string Render(DefEnum c)
        {
            return RenderUtil.RenderCppEnumClass(c);
        }

        public string RenderForwardDefine(DefBean b)
        {
            return $"{b.CppNamespaceBegin} class {b.Name}; {b.CppNamespaceEnd} ";
        }

        public override string Render(DefBean b)
        {
            var template = StringTemplateUtil.GetTemplate("config/cpp_bin/bean");
            var result = template.RenderCode(b);
            return result;
        }

        public override string Render(DefTable p)
        {
            var template = StringTemplateUtil.GetTemplate("config/cpp_bin/table");
            var result = template.RenderCode(p);
            return result;
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = StringTemplateUtil.GetTemplate("config/cpp_bin/tables");
            var result = template.Render(new
            {
                Name = name,
                Tables = tables,
            });
            return result;
        }

        public string RenderStub(string topModule, List<DefTypeBase> types)
        {
            var template = StringTemplateUtil.GetTemplate("config/cpp_bin/stub");
            return template.RenderCode(new
            {
                TopModule = topModule,
                Types = types,
            });
        }
    }
}
