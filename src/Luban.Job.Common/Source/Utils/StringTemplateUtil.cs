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
        private static List<string> TemplateSearchPaths { get; } = new List<string>();

        public static void AddTemplateSearchPath(string path)
        {
            TemplateSearchPaths.Add(path);
        }

        public static bool TryGetTemplateString(string templateName, out string result)
        {
            foreach (var searchPath in TemplateSearchPaths)
            {
                var fullPath = $"{searchPath}/{templateName}.tpl";
                if (File.Exists(fullPath))
                {
                    result = File.ReadAllText(fullPath, Encoding.UTF8);
                    return true;
                }
            }
            result = null;
            return false;
        }

        public static string GetTemplateString(string templateName)
        {
            if (TryGetTemplateString(templateName, out var strTpl))
            {
                return strTpl;
            }
            throw new FileNotFoundException($"can't find {templateName}.tpl in paths:{string.Join(';', TemplateSearchPaths)}");
        }

        private static readonly ConcurrentDictionary<string, Template> s_templates = new();

        public static Template GetTemplate(string templateName)
        {
            return s_templates.GetOrAdd(templateName, tn => Template.Parse(GetTemplateString(tn)));
        }

        public static bool TryGetTemplate(string templateName, out Template template)
        {
            if (s_templates.TryGetValue(templateName, out template))
            {
                return true;
            }
            if (TryGetTemplateString(templateName, out var strTpm))
            {
                template = s_templates.GetOrAdd(templateName, tn => Template.Parse(strTpm));
                return true;
            }
            template = null;
            return false;
        }

        public static Template GetOrAddTemplate(string templateName, Func<string, Template> creator)
        {
            return s_templates.GetOrAdd(templateName, creator);
        }
    }
}
