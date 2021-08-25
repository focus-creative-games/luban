using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class CppCodeBinRender : CodeRenderBase
    {
        public override string Render(DefConst c)
        {
            return RenderUtil.RenderCppConstClass(c);
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
