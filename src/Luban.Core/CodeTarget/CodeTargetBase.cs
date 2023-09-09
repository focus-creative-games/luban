using System.Reflection;
using Luban.CodeFormat;
using Luban.CodeFormat.CodeStyles;
using Luban.Defs;

namespace Luban.CodeTarget;

public abstract class CodeTargetBase : ICodeTarget
{
    public const string FamilyPrefix = "codeTarget";

    protected virtual string GetFileNameWithoutExtByTypeName(string name)
    {
        return name;
    }

    protected virtual ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.NoneCodeStyle;

    protected virtual ICodeStyle CodeStyle => _codeStyle ??= CreateConfigurableCodeStyle();

    private ICodeStyle _codeStyle;

    private ICodeStyle CreateConfigurableCodeStyle()
    {
        var baseStyle = GenerationContext.Current.GetCodeStyle(Name) ?? DefaultCodeStyle;

        var env = EnvManager.Current;
        string namingKey = BuiltinOptionNames.NamingConvention;
        return new OverlayCodeStyle(baseStyle,
            env.GetOptionOrDefault($"{namingKey}.{Name}", "namespace", true, ""),
            env.GetOptionOrDefault($"{namingKey}.{Name}", "type", true, ""),
            env.GetOptionOrDefault($"{namingKey}.{Name}", "method", true, ""),
            env.GetOptionOrDefault($"{namingKey}.{Name}", "property", true, ""),
            env.GetOptionOrDefault($"{namingKey}.{Name}", "field", true, ""),
            env.GetOptionOrDefault($"{namingKey}.{Name}", "enumItem", true, "")
            );
    }

    public virtual void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.ExportTables, writer);
            return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(ctx.Target.Manager)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
        }));

        foreach (var table in ctx.ExportTables)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateTable(ctx, table, writer);
                return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(table.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(bean.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(@enum.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }

    public string Name => GetType().GetCustomAttribute<CodeTargetAttribute>().Name;

    public abstract string FileHeader { get; }
    
    protected abstract string FileSuffixName { get; }
    
    protected void AddCodeFile(OutputFileManifest manifest, string fullName, CodeWriter writer)
    {
        var path = GetPathFromFullName(fullName);
        manifest.AddFile(path, writer.ToResult(FileHeader));
    }

    public virtual string GetPathFromFullName(string fullName)
    {
        return fullName.Replace('.', '/') + "." + FileSuffixName;
    }
    
    public abstract void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer);
    public abstract void GenerateTable(GenerationContext ctx, DefTable table, CodeWriter writer);
    public abstract void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer);
    public abstract void GenerateEnum(GenerationContext ctx, DefEnum @enum, CodeWriter writer);
}