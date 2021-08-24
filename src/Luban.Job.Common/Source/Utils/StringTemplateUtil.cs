using System;
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
    }
}
