using Scriban;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Utils
{
    public static class StringTemplateUtil
    {
        public static string TemplateDir { get; set; }

        public static string GetTemplateString(string templateName)
        {
            return File.ReadAllText($"{TemplateDir}/{templateName}.tpl", Encoding.UTF8);
        }

        private static readonly ConcurrentDictionary<string, Template> s_templates = new();

        public static Template GetTemplate(string templateName)
        {
            return s_templates.GetOrAdd(templateName, tn => Template.Parse(GetTemplateString(tn)));
        }

        public static Template GetOrAddTemplate(string templateName, Func<string, Template> creator)
        {
            return s_templates.GetOrAdd(templateName, creator);
        }
    }
}
