using Scriban;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Tpl
{
    public class StringTemplateManager
    {
        public static StringTemplateManager Ins { get; } = new();


        private readonly List<string> _templateSearchPaths = new();

        private bool _enableTemplateCache;

        private readonly ConcurrentDictionary<string, Template> _templates = new();

        private readonly ConcurrentDictionary<string, string> _templateStrings = new();

        public void Init(bool enableTemplateCache)
        {
            _enableTemplateCache = enableTemplateCache;
        }

        public void AddTemplateSearchPath(string templateSearchPath)
        {
            _templateSearchPaths.Add(templateSearchPath);
        }

        public bool TryGetTemplateString(string templateName, out string result)
        {
            if (_enableTemplateCache && _templateStrings.TryGetValue(templateName, out result))
            {
                return true;
            }
            foreach (var searchPath in _templateSearchPaths)
            {
                var fullPath = $"{searchPath}/{templateName}.tpl";
                if (File.Exists(fullPath))
                {
                    result = File.ReadAllText(fullPath, Encoding.UTF8);
                    if (_enableTemplateCache)
                    {
                        _templateStrings[templateName] = result;
                    }
                    return true;
                }
            }
            result = null;
            return false;
        }

        public string GetTemplateString(string templateName)
        {
            if (TryGetTemplateString(templateName, out var strTpl))
            {
                return strTpl;
            }
            throw new FileNotFoundException($"can't find {templateName}.tpl in paths:{string.Join(';', _templateSearchPaths)}");
        }

        public Template GetTemplate(string templateName)
        {
            if (_enableTemplateCache)
            {
                return _templates.GetOrAdd(templateName, tn => Template.Parse(GetTemplateString(tn)));
            }
            else
            {
                return Template.Parse(GetTemplateString(templateName));
            }
        }

        public bool TryGetTemplate(string templateName, out Template template)
        {
            if (_templates.TryGetValue(templateName, out template))
            {
                return true;
            }
            if (TryGetTemplateString(templateName, out var strTpm))
            {
                template = _templates.GetOrAdd(templateName, tn => Template.Parse(strTpm));
                return true;
            }
            template = null;
            return false;
        }

        public Template GetOrAddTemplate(string templateName, Func<string, Template> creator)
        {
            return _templates.GetOrAdd(templateName, creator);
        }
    }
}
