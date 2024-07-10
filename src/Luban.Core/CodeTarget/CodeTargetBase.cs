using System.Reflection;
using System.Text;
using Luban.CodeFormat;
using Luban.CodeFormat.CodeStyles;
using Luban.Defs;

namespace Luban.CodeTarget;

public abstract class CodeTargetBase : ICodeTarget
{
    public const string FamilyPrefix = "codeTarget";

    public virtual Encoding FileEncoding
    {
        get
        {
            string encoding = EnvManager.Current.GetOptionOrDefault(Name, BuiltinOptionNames.FileEncoding, true, "");
            return string.IsNullOrEmpty(encoding) ? Encoding.UTF8 : System.Text.Encoding.GetEncoding(encoding);
        }
    }

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

    protected abstract IReadOnlySet<string> PreservedKeyWords { get; }

    protected bool IsPreserveKeyWords(string name)
    {
        return PreservedKeyWords.Contains(name);
    }

    protected virtual bool IsValidateName(string name, NameLocation location)
    {
        return !string.IsNullOrEmpty(name);
    }

    protected virtual void ValidationTypeNames(GenerationContext ctx)
    {
        foreach (var table in ctx.ExportTables)
        {
            if (IsPreserveKeyWords(table.Name))
            {
                throw new Exception($"table name {table.FullName} is preserved keyword");
            }
            if (!IsValidateName(table.Name, NameLocation.TableName))
            {
                throw new Exception($"table name {table.FullName} is invalid");
            }
        }

        foreach (var bean in ctx.ExportBeans)
        {
            if (IsPreserveKeyWords(bean.Name))
            {
                throw new Exception($"bean name {bean.FullName} is preserved keyword");
            }
            if (!IsValidateName(bean.Name, NameLocation.BeanName))
            {
                throw new Exception($"bean name {bean.FullName}  is invalid");
            }
            foreach (var field in bean.Fields)
            {
                if (IsPreserveKeyWords(field.Name))
                {
                    throw new Exception($"the name of field {bean.FullName}::{field.Name} is preserved keyword");
                }
                if (!IsValidateName(field.Name, NameLocation.BeanFieldName))
                {
                    throw new Exception($"the name of field {bean.FullName}::{field.Name} is invalid");
                }
            }
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            if (IsPreserveKeyWords(@enum.Name))
            {
                throw new Exception($"enum name {@enum.FullName} is preserved keyword");
            }
            if (!IsValidateName(@enum.Name, NameLocation.EnumName))
            {
                throw new Exception($"enum name {@enum.FullName}  is invalid");
            }
            foreach (var item in @enum.Items)
            {
                if (IsPreserveKeyWords(item.Name))
                {
                    throw new Exception($"the name of enum item '{@enum.FullName}::{item.Name}' is preserved keyword");
                }
                if (!IsValidateName(item.Name, NameLocation.EnumItemName))
                {
                    throw new Exception($"the name of enum item '{@enum.FullName}::{item.Name}' is invalid");
                }
            }
        }
    }   

    public virtual void ValidateDefinition(GenerationContext ctx)
    {
        ValidationTypeNames(ctx);
    }

    public virtual void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.ExportTables, writer);
            return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(ctx.Target.Manager)}.{FileSuffixName}", writer.ToResult(FileHeader));
        }));

        foreach (var table in ctx.ExportTables)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateTable(ctx, table, writer);
                return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(table.FullName)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(bean.FullName)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(@enum.FullName)}.{FileSuffixName}", writer.ToResult(FileHeader));
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

    public virtual string GetPathFromFullName(string fullName)
    {
        return fullName.Replace('.', '/') + "." + FileSuffixName;
    }

    protected OutputFile CreateOutputFile(string path, string content)
    {
        return new OutputFile() { File = path, Content = content, Encoding = FileEncoding };
    }

    public abstract void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer);
    public abstract void GenerateTable(GenerationContext ctx, DefTable table, CodeWriter writer);
    public abstract void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer);
    public abstract void GenerateEnum(GenerationContext ctx, DefEnum @enum, CodeWriter writer);
}
