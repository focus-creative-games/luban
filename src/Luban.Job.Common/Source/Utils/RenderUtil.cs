using Luban.Job.Common.Defs;
using Scriban;
using System;

namespace Luban.Job.Common.Utils
{
    public static class RenderUtil
    {
        [ThreadStatic]
        private static Template t_constRender;
        public static string RenderCsConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);

            var template = t_constRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/cs/const"));
            var result = template.Render(ctx);

            return result;
        }

        [ThreadStatic]
        private static Template t_enumRender;
        public static string RenderCsEnumClass(DefEnum e)
        {
            var template = t_enumRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/cs/enum"));
            var result = template.Render(e);

            return result;
        }

        [ThreadStatic]
        private static Template t_javaConstRender;
        public static string RenderJavaConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = t_javaConstRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/java/const"));
            var result = template.Render(ctx);

            return result;
        }

        [ThreadStatic]
        private static Template t_javaEnumRender;
        public static string RenderJavaEnumClass(DefEnum e)
        {
            var template = t_javaEnumRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/java/enum"));
            var result = template.Render(e);

            return result;
        }

        [ThreadStatic]
        private static Template t_cppConstRender;
        public static string RenderCppConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = t_cppConstRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/cpp/const"));
            var result = template.Render(ctx);
            return result;
        }

        [ThreadStatic]
        private static Template t_cppEnumRender;
        public static string RenderCppEnumClass(DefEnum e)
        {
            var template = t_cppEnumRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/cpp/enum"));
            var result = template.Render(e);
            return result;
        }

        [ThreadStatic]
        private static Template t_pythonConstRender;
        public static string RenderPythonConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = t_pythonConstRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/python/const"));
            var result = template.Render(ctx);

            return result;
        }

        [ThreadStatic]
        private static Template t_pythonEnumRender;
        public static string RenderPythonEnumClass(DefEnum e)
        {
            var template = t_pythonEnumRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/python/enum"));
            var result = template.Render(e);

            return result;
        }

        [ThreadStatic]
        private static Template t_tsConstRender;
        public static string RenderTypescriptConstClass(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = t_tsConstRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/typescript/const"));
            var result = template.Render(ctx);

            return result;
        }

        [ThreadStatic]
        private static Template t_tsEnumRender;
        public static string RenderTypescriptEnumClass(DefEnum e)
        {
            var template = t_tsEnumRender ??= Template.Parse(StringTemplateUtil.GetTemplateString("common/typescript/enum"));
            var result = template.Render(e);

            return result;
        }
    }
}
