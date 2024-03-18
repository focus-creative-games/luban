using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Cpp.TemplateExtensions;
using Luban.Defs;
using Scriban;
using Scriban.Runtime;
using Luban.Utils;

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

    private void PrintBean(GenerationContext ctx)
    {
        foreach (var @bean in ctx.ExportBeans)
        {
            Console.WriteLine("Name:{0}", bean.Name);
            
            foreach (var @Field in @bean.ExportFields)
            {
                Console.WriteLine("Field:{0}, is bean[{1}]", Field.Name, Field.CType.IsBean);
            }
        }
    }

    public void GenerateTablesCpp(GenerationContext ctx, List<DefTable> tables, CodeWriter writer)
    {
        var template = GetTemplate("tables_cpp");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager },
            { "__namespace", ctx.Target.TopModule },
            { "__tables", tables },
            { "__code_style", CodeStyle},
            { "__tables_count", tables.Count }
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        // enum
        {
            var writer = new CodeWriter();
            writer.Write("#pragma once");
        
            foreach (var @enum in ctx.ExportEnums)
            {
                base.GenerateEnum(ctx, @enum, writer);
            }
            writer.ToResult(null);
            manifest.AddFile(new OutputFile() { File = "enum.h", Content = writer.ToResult(FileHeader) });
        }
        
        // beans
        {
            foreach (var @bean in ctx.ExportBeans)
            {
                var writer = new CodeWriter();
                writer.Write("#pragma once");
                writer.Write("#include \"enum.h\"");
                writer.Write("#include \"CfgBean.h\"");
            
                base.GenerateBean(ctx, @bean, writer);
                manifest.AddFile(new OutputFile() { File = $"{TypeUtil.ToSnakeCase(bean.FullName)}.h", Content = writer.ToResult(FileHeader) });
            }
        }
        
        
        // table
        {
            foreach (var @table in ctx.ExportTables)
            {
                var writer = new CodeWriter();
                writer.Write("#pragma once");
            
                base.GenerateTable(ctx, @table, writer);
                manifest.AddFile(new OutputFile() { File = $"{TypeUtil.ToSnakeCase(table.Name)}.h", Content = writer.ToResult(FileHeader) });
            }
        }

        // tables
        {
            var writer = new CodeWriter();
            writer.Write("#pragma once");
            
            base.GenerateTables(ctx, ctx.ExportTables, writer);
        
            manifest.AddFile(new OutputFile() { File = "tables.h", Content = writer.ToResult(FileHeader) });
        }

        // tables.cpp
        {
            var writer = new CodeWriter();
            writer.Write("#include \"tables.h\"");
            
            GenerateTablesCpp(ctx, ctx.ExportTables, writer);
        
            manifest.AddFile(new OutputFile() { File = "tables.cpp", Content = writer.ToResult(FileHeader) });
        }

        // debug
        {
            PrintBean(ctx);
        }
    }
}
