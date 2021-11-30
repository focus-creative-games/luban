using Luban.Job.Common.Defs;
using Luban.Job.Common.Tpl;
using Scriban;

namespace Luban.Job.Common.Utils
{
    public static class RenderUtil
    {
        public static string RenderCsEnumClass(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/cs/enum");
            var result = template.Render(e);

            return result;
        }

        public static string RenderJavaEnumClass(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/java/enum");
            var result = template.Render(e);

            return result;
        }

        public static string RenderCppEnumClass(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/cpp/enum");
            var result = template.Render(e);
            return result;
        }

        public static string RenderPythonEnumClass(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/python/enum");
            var result = template.Render(e);

            return result;
        }

        public static string RenderTypescriptEnumClass(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/typescript/enum");
            var result = template.Render(e);
            return result;
        }

        public static string RenderRustEnumClass(DefEnum e)
        {
            var template = StringTemplateManager.Ins.GetTemplate("common/rust/enum");
            var result = template.Render(e);

            return result;
        }
    }
}
