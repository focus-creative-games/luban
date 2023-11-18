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

        return new OutputFile() { File = outputFileName, Content = schemaHeader.ToResult(FileHeader) };
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

        return new OutputFile() { File = outputFileName, Content = schemaCpp.ToResult(FileHeader) };
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
