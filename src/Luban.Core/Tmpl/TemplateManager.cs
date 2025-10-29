// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

﻿using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Scriban;

namespace Luban.Tmpl;

public class TemplateManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static TemplateManager Ins { get; } = new();


    private readonly List<string> _templateSearchPaths = new();

    private readonly ConcurrentDictionary<string, Template> _templates = new();

    private readonly ConcurrentDictionary<string, string> _templateStrings = new();

    public void Init()
    {
        string curDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        AddTemplateSearchPath($"{curDir}/Templates", true);
    }

    public void AddTemplateSearchPath(string templateSearchPath, bool sureExists = false, bool addFirst = false)
    {
        if (!Directory.Exists(templateSearchPath))
        {
            if (sureExists)
            {
                s_logger.Error("template search path:{} not exists", templateSearchPath);
            }
            return;
        }
        if (addFirst)
        {
            _templateSearchPaths.Insert(0, templateSearchPath);
        }
        else
        {
            _templateSearchPaths.Add(templateSearchPath);
        }
    }

    public bool TryGetTemplateString(string templateName, out string result)
    {
        if (_templateStrings.TryGetValue(templateName, out result))
        {
            return true;
        }
        foreach (var searchPath in _templateSearchPaths)
        {
            var fullPath = $"{searchPath}/{templateName}.sbn";
            if (File.Exists(fullPath))
            {
                result = File.ReadAllText(fullPath, Encoding.UTF8);
                _templateStrings[templateName] = result;
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
        throw new FileNotFoundException($"can't find {templateName}.sbn in paths:{string.Join(';', _templateSearchPaths)}");
    }

    public Scriban.Template GetTemplate(string templateName)
    {
        return _templates.GetOrAdd(templateName, tn => Template.Parse(GetTemplateString(tn)));
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
