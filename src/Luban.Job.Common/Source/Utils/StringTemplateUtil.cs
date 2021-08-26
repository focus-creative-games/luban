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

        public static string GetTemplateString(string templateName)
        {
            foreach (var searchPath in TemplateSearchPaths)
            {
                var fullPath = $"{searchPath}/{templateName}.tpl";
                if (File.Exists(fullPath))
                {
                    return File.ReadAllText(fullPath, Encoding.UTF8);
                }
            }
            throw new FileNotFoundException($"can't find {templateName}.tpl in paths:{string.Join(';', TemplateSearchPaths)}");
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
