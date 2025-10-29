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

﻿using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Defs;
using Luban.Rust.TemplateExtensions;
using Luban.Tmpl;
using Luban.Utils;

using Scriban;
using Scriban.Runtime;
using System.Text;
using System.Xml.Linq;

namespace Luban.Rust.CodeTarget;

public class RustCodeTargetBase : TemplateCodeTargetBase
{
    protected override IReadOnlySet<string> PreservedKeyWords => _preservedKeyWords;
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_RUST;
    protected override string FileSuffixName => "rs";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.PythonDefaultCodeStyle;

    private static readonly HashSet<string> _preservedKeyWords =
    [
        "as", "async", "await", "break", "const", "continue", "crate",
        "dyn", "else", "enum", "extern", "false", "fn", "for", "if",
        "impl", "in", "let", "loop", "match", "mod", "move", "mut",
        "pub", "ref", "return", "self", "Self", "static", "struct",
        "super", "trait", "true", "type", "union", "unsafe", "use",
        "where", "while",
        // Keywords Reserved for Future Use
        "abstract", "become", "box", "do", "final", "macro", "override",
        "priv", "try", "typeof", "unsized", "virtual", "yield"
    ];

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new RustCommonTemplateExtension());
    }


    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var topMod = new Mod();
        var modDic = new Dictionary<string, Mod>();
        var polymorphicBeans = new HashSet<DefBean>();
        foreach (var item in ctx.ExportTables)
        {
            CollectMod(item, topMod, modDic);
        }

        foreach (var item in ctx.ExportBeans)
        {
            if (item.IsAbstractType)
            {
                polymorphicBeans.Add(item);
            }
            if (!string.IsNullOrEmpty(item.Parent))
            {
                polymorphicBeans.Add(item);
            }
            CollectMod(item, topMod, modDic);
        }

        foreach (var item in ctx.ExportEnums)
        {
            CollectMod(item, topMod, modDic);
        }

        var tasks = new List<Task<OutputFile>>
        {
            Task.Run(() =>
            {
                var allns = modDic.Values.Select(x => "crate::" + x.FullPath.Replace("/", "::")).ToList();
                var allmods = modDic.Keys.Select(x => x.Replace(".", "::"));
                return CreateOutputFile($"{GenerationContext.Current.TopModule}/src/lib.rs", GenerateLib(ctx, allmods, allns, topMod, polymorphicBeans));
            }),
            Task.Run(() => CreateOutputFile($"{GenerationContext.Current.TopModule}/Cargo.toml", GenerateToml(ctx))),
        };

        foreach (var mod in modDic.Values)
        {
            tasks.Add(Task.Run(() =>
            {
                var path = $"{GenerationContext.Current.TopModule}/src/{mod.FullPath}";
                path += mod.SubMods.Count <= 0 ? ".rs" : "/mod.rs";

                return CreateOutputFile(path, GenerateMod(ctx, mod));
            }));
        }

        GenerateMacros(ctx, manifest);
        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }

    private static void CollectMod<T>(T item, Mod topMod, Dictionary<string, Mod> modDic) where T : DefTypeBase
    {
        Mod mod = topMod;
        if (!string.IsNullOrEmpty(item.Namespace))
        {
            var ns = item.Namespace.Split(".");
            var parent = topMod;
            foreach (var se in ns)
            {
                if (!modDic.TryGetValue(se, out mod!))
                {
                    mod = new Mod
                    {
                        Name = se
                    };

                    mod.FullPath = string.IsNullOrEmpty(parent.FullPath)
                        ? mod.Name
                        : $"{parent.FullPath}/{mod.Name}";

                    modDic.Add(se, mod);
                }

                parent.SubMods.Add(mod);
                parent = mod;
            }
        }

        switch (item)
        {
            case DefTable def:
                mod.Tables.Add(def);
                break;
            case DefBean def:
                mod.Beans.Add(def);
                break;
            case DefEnum def:
                mod.Enums.Add(def);
                break;
        }
    }

    protected virtual string GenerateToml(GenerationContext ctx)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"toml");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            {"__name", GenerationContext.Current.TopModule},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        return writer.ToResult(string.Empty);
    }

    protected virtual string GenerateLib(GenerationContext ctx, IEnumerable<string> mods, IEnumerable<string> ns, Mod topMod, HashSet<DefBean> polymorphicBeans)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"lib");
        var tplCtx = CreateTemplateContext(template);
        OnCreateTemplateContext(tplCtx);

        var extraEnvs = new ScriptObject
        {
            {"__ctx", ctx},
            {"__name", ctx.Target.Manager},
            {"__namespace", ctx.Target.TopModule},
            {"__full_name", TypeUtil.MakeFullName(ctx.Target.TopModule, ctx.Target.Manager)},
            {"__top_mod", topMod},
            {"__mods", mods},
            {"__ns", ns},
            {"__tables", ctx.ExportTables},
            {"__polymorphic_beans", polymorphicBeans},
            {"__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        var result = writer.ToResult(FileHeader);
        result = result + GenerateMod(ctx, topMod, false);
        return result;
    }

    protected virtual string GenerateMod(GenerationContext ctx, Mod mod, bool addHeader = true)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"mod");
        var tplCtx = CreateTemplateContext(template);
        OnCreateTemplateContext(tplCtx);

        var extraEnvs = new ScriptObject
        {
            {"__ctx", ctx},
            {"__name", ctx.Target.Manager},
            {"__namespace", ctx.Target.TopModule},
            {"__full_name", TypeUtil.MakeFullName(ctx.Target.TopModule, ctx.Target.Manager)},
            {"__mod", mod},
            {"__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        var result = writer.ToResult(addHeader ? FileHeader : null);
        return result;
    }

    protected virtual void GenerateMacros(GenerationContext ctx, OutputFileManifest manifest)
    {
        var template = TemplateManager.Ins.GetTemplateString($"{CommonTemplateSearchPath}/macros/Cargo.toml");
        var path = $"macros/Cargo.toml";
        manifest.AddFile(CreateOutputFile(path, template));
        template = TemplateManager.Ins.GetTemplateString($"{CommonTemplateSearchPath}/macros/src/lib.rs");
        path = $"macros/src/lib.rs";
        manifest.AddFile(CreateOutputFile(path, template));
    }

    protected class Mod
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public HashSet<Mod> SubMods { get; set; } = [];
        public List<DefTable> Tables { get; set; } = [];
        public List<DefBean> Beans { get; set; } = [];
        public List<DefEnum> Enums { get; set; } = [];
    }
}
