using Luban.CodeTarget;
using Luban.Datas;
using Luban.Defs;
using Luban.Document.TemplateExtensions;
using Luban.TemplateExtensions;
using Luban.Types;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;
using System.Text;

namespace Luban.Document.CodeTarget;

[CodeTarget("doc")]
public class DocumentCodeTarget : TemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override IReadOnlySet<string> PreservedKeyWords => new HashSet<string>();

    protected override string FileSuffixName => "md";



    public override void ValidateDefinition(GenerationContext ctx)
    {
        base.ValidateDefinition(ctx);

        foreach (var table in ctx.ExportTables)
        {
            ResolveTableReference(table);
        }

        foreach (var bean in ctx.ExportBeans)
        {
            ResolveBeanReference(bean);
        }
    }

    private void ResolveTableReference(DefTable table)
    {
        var valueType = table.ValueTType.DefBean;
        DocumentTemplateExtensions.AddReference(valueType, table);
    }

    private void ResolveBeanReference(DefBean bean)
    {
        // 引用父类
        var parent = bean.ParentDefType;
        if (parent != null)
        {
            DocumentTemplateExtensions.AddReference(parent, bean);
        }

        foreach (var field in bean.ExportFields)
        {
            ResolveType(field.CType);

            // 引用ref表
            if (TypeTemplateExtension.CanGenerateCollectionRef(field))
            {
                var refTable = TypeTemplateExtension.GetCollectionRefTable(field);
                DocumentTemplateExtensions.AddReference(refTable, bean);
            }
            else if (TypeTemplateExtension.CanGenerateRef(field))
            {
                var refTable = TypeTemplateExtension.GetRefTable(field);
                DocumentTemplateExtensions.AddReference(refTable, bean);
            }
        }

        void ResolveType(TType type)
        {
            if (type.IsBean)
            {
                var beanDef = (type as TBean).DefBean;
                DocumentTemplateExtensions.AddReference(beanDef, bean);
            }
            if (type.IsEnum)
            {
                var enumDef = (type as TEnum).DefEnum;
                DocumentTemplateExtensions.AddReference(enumDef, bean);
            }
            if (type.IsCollection)
            {
                if (type is TList or TArray or TSet)
                {
                    var elementType = type.ElementType;
                    ResolveType(elementType);
                }
                if (type is TMap map)
                {
                    var keyType = map.KeyType;
                    var valueType = map.ValueType;
                    ResolveType(keyType);
                    ResolveType(valueType);
                }
            }
        }
    }
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new DocumentTemplateExtensions());
    }



    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.ExportTables, writer);
            return CreateOutputFile($"@{ctx.Target.Manager}.{FileSuffixName}", writer.ToResult(FileHeader));
        }));

        foreach (var table in ctx.ExportTables)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateTable(ctx, table, writer);
                return CreateOutputFile($"{DocumentTemplateExtensions.GetDefDocumentPath(table)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return CreateOutputFile($"{DocumentTemplateExtensions.GetDefDocumentPath(bean)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return CreateOutputFile($"{DocumentTemplateExtensions.GetDefDocumentPath(@enum)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }
}
