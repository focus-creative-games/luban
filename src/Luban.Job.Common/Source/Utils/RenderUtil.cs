using Luban.Job.Common.Defs;
using Scriban;

namespace Luban.Job.Common.Utils
{
    public static class RenderUtil
    {
        public static string RenderCsConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);

            var template = StringTemplateUtil.GetTemplate("common/cs/const");
            var result = template.Render(ctx);

            return result;
        }

        public static string RenderCsEnumClass(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate("common/cs/enum");
            var result = template.Render(e);

            return result;
        }

        public static string RenderJavaConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = StringTemplateUtil.GetTemplate("common/java/const");
            var result = template.Render(ctx);

            return result;
        }

        public static string RenderJavaEnumClass(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate("common/java/enum");
            var result = template.Render(e);

            return result;
        }

        public static string RenderCppConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = StringTemplateUtil.GetTemplate("common/cpp/const");
            var result = template.Render(ctx);
            return result;
        }

        public static string RenderCppEnumClass(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate("common/cpp/enum");
            var result = template.Render(e);
            return result;
        }

        public static string RenderPythonConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = StringTemplateUtil.GetTemplate("common/python/const");
            var result = template.Render(ctx);

            return result;
        }

        public static string RenderPythonEnumClass(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate("common/python/enum");
            var result = template.Render(e);

            return result;
        }

        public static string RenderTypescriptConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = StringTemplateUtil.GetTemplate("common/typescript/const");
            var result = template.Render(ctx);

            return result;
        }

        public static string RenderTypescriptEnumClass(DefEnum e)
        {
            var template = StringTemplateUtil.GetTemplate("common/typescript/enum");
            var result = template.Render(e);

            return result;
        }
    }
}
