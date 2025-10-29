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

using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Cpp.TemplateExtensions;
using Luban.Defs;
using Scriban;
using Scriban.Runtime;

namespace Luban.Cpp.CodeTarget;

public abstract class CppCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "cpp";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.CppDefaultCodeStyle;

    private static readonly HashSet<string> s_preservedKeyWords = new HashSet<string>
    {
        // cpp preserved key words
        "alignas", "alignof", "and", "and_eq", "asm", "atomic_cancel", "atomic_commit", "atomic_noexcept",
        "auto", "bitand", "bitor", "bool", "break", "case", "catch", "char", "char8_t", "char16_t", "char32_t",
        "class", "compl", "concept", "const", "consteval", "constexpr", "constinit", "const_cast", "continue",
        "co_await", "co_return", "co_yield", "decltype", "default", "delete", "do", "double", "dynamic_cast",
        "else", "enum", "explicit", "export", "extern", "false", "float", "for", "friend", "goto", "if", "import",
        "inline", "int", "long", "module", "mutable", "namespace", "new", "noexcept", "not", "not_eq", "nullptr",
        "operator", "or", "or_eq", "private", "protected", "public", "reflexpr", "register", "reinterpret_cast",
        "requires", "return", "short", "signed", "sizeof", "static", "static_assert", "static_cast", "struct",
        "switch", "synchronized", "template", "this", "thread_local", "throw", "true", "try", "typedef", "typeid",
        "typename", "union", "unsigned", "using", "virtual", "void", "volatile", "wchar_t", "while", "xor", "xor_eq"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new CppTemplateExtension());
    }

    private OutputFile GenerateSchemaHeader(GenerationContext ctx, string outputFileName)
    {
        var enumTasks = new List<Task<string>>();
        foreach (var @enum in ctx.ExportEnums)
        {
            enumTasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return writer.ToResult(null);
            }));
        }

        var beanTasks = new List<Task<string>>();
        foreach (var bean in ctx.ExportBeans)
        {
            beanTasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return writer.ToResult(null);
            }));
        }

        var tableTasks = new List<Task<string>>();
        foreach (var table in ctx.ExportTables)
        {
            tableTasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateTable(ctx, table, writer);
                return writer.ToResult(null);
            }));
        }

        var tablesWriter = new CodeWriter();
        GenerateTables(ctx, ctx.ExportTables, tablesWriter);


        Task.WaitAll(enumTasks.ToArray());
        Task.WaitAll(beanTasks.ToArray());
        Task.WaitAll(tableTasks.ToArray());

        var template = GetTemplate("schema_h");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", ctx.Target.TopModule },
            { "__enum_codes", string.Join('\n', enumTasks.Select(t => t.Result))},
            { "__bean_codes", string.Join('\n', beanTasks.Select(t => t.Result))},
            { "__table_codes", string.Join('\n', tableTasks.Select(t => t.Result))},
            { "__tables_code", tablesWriter.ToResult(null)},
            { "__beans", ctx.ExportBeans},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        var schemaHeader = new CodeWriter();
        schemaHeader.Write(template.Render(tplCtx));

        return CreateOutputFile(outputFileName, schemaHeader.ToResult(FileHeader));
    }

    private OutputFile GenerateSchemaCpp(GenerationContext ctx, List<DefBean> beans, string schemaHeaderFileName, string outputFileName)
    {
        var template = GetTemplate("schema_cpp");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", ctx.Target.TopModule },
            { "__beans", beans},
            { "__schema_header_file", schemaHeaderFileName},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        var schemaCpp = new CodeWriter();
        schemaCpp.Write(template.Render(tplCtx));

        return CreateOutputFile(outputFileName, schemaCpp.ToResult(FileHeader));
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        string schemaFileNameWithoutExt = EnvManager.Current.GetOptionOrDefault(Name, "schemaFileNameWithoutExt", true, "schema");
        string schemaFileName = $"{schemaFileNameWithoutExt}.h";
        manifest.AddFile(GenerateSchemaHeader(ctx, schemaFileName));

        var cppTasks = new List<Task<OutputFile>>();
        var beanTypes = ctx.ExportBeans;

        int typeCountPerStubFile = int.Parse(EnvManager.Current.GetOptionOrDefault(Name, "typeCountPerStubFile", true, "100"));

        for (int i = 0, n = beanTypes.Count; i < n; i += typeCountPerStubFile)
        {
            int startIndex = i;
            cppTasks.Add(Task.Run(() =>
                GenerateSchemaCpp(ctx,
                    beanTypes.GetRange(startIndex, Math.Min(typeCountPerStubFile, beanTypes.Count - startIndex)),
                    schemaFileName,
                    $"{schemaFileNameWithoutExt}_{startIndex / typeCountPerStubFile}.cpp")));
        }

        Task.WaitAll(cppTasks.ToArray());
        foreach (var cppTask in cppTasks)
        {
            manifest.AddFile(cppTask.Result);
        }
    }
}
