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

    private void GenerateTablesCpp(GenerationContext ctx, List<DefTable> tables, CodeWriter writer)
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
    
    private void GenerateAbstractBeanDeserialize(GenerationContext ctx, List<DefBean> beans, CodeWriter writer)
    {
        var template = GetTemplate("abstract_bean_deserialize");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager },
            { "__namespace", ctx.Target.TopModule },
            { "__manager_name_with_top_module", TypeUtil.MakeFullName(ctx.TopModule, ctx.Target.Manager) },
            { "__beans", beans },
            { "__code_style", CodeStyle}
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();

        // enum
        {
            var writer = new CodeWriter();
            writer.Write("#pragma once");
        
            foreach (var @enum in ctx.ExportEnums)
            {
                tasks.Add(Task.Run(() =>
                {
                    base.GenerateEnum(ctx, @enum, writer);
                    return new OutputFile() { File = "enum.h", Content = writer.ToResult(FileHeader) };
                }));
            }
        }
        
        // beans
        {
            foreach (var @bean in ctx.ExportBeans)
            {
                tasks.Add(Task.Run(() =>
                {
                    var writer = new CodeWriter();
                    writer.Write("#pragma once");
                    writer.Write("#include \"enum.h\"");
                    writer.Write("#include \"CfgBean.h\"");
                
                    base.GenerateBean(ctx, @bean, writer);
                    return new OutputFile() { File = $"{TypeUtil.ToSnakeCase(bean.FullName)}.h", Content = writer.ToResult(FileHeader) };
                }));
            }
        }

        // abstract bean deserialize
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateAbstractBeanDeserialize(ctx, ctx.ExportBeans, writer);
                return new OutputFile() { File = $"abstract_bean_deserialize.cpp", Content = writer.ToResult(FileHeader) };
            }));
        }


        // table
        {
            foreach (var @table in ctx.ExportTables)
            {
                tasks.Add(Task.Run(() =>
                {
                    var writer = new CodeWriter();
                    writer.Write("#pragma once");
                
                    base.GenerateTable(ctx, @table, writer);
                    return new OutputFile() { File = $"{TypeUtil.ToSnakeCase(table.Name)}.h", Content = writer.ToResult(FileHeader) };
                }));
            }
        }

        // tables
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                writer.Write("#pragma once");
                
                base.GenerateTables(ctx, ctx.ExportTables, writer);
            
                return new OutputFile() { File = "tables.h", Content = writer.ToResult(FileHeader) };
            }));
        }

        // tables.cpp
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                writer.Write("#include \"tables.h\"");

                GenerateTablesCpp(ctx, ctx.ExportTables, writer);
                return new OutputFile() { File = "tables.cpp", Content = writer.ToResult(FileHeader) };
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }

        // debug
        {
            // PrintBean(ctx);
        }
    }
}
