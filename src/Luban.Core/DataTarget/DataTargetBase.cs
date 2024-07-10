using Luban.CodeTarget;
using Luban.Defs;
using System.Reflection;
using System.Text;

namespace Luban.DataTarget;

public abstract class DataTargetBase : IDataTarget
{
    public const string FamilyPrefix = "tableExporter";

    public string Name => GetType().GetCustomAttribute<DataTargetAttribute>().Name;

    public virtual Encoding FileEncoding
    {
        get
        {
            string encoding = EnvManager.Current.GetOptionOrDefault(Name, BuiltinOptionNames.FileEncoding, true, "");
            return string.IsNullOrEmpty(encoding) ? Encoding.UTF8 : System.Text.Encoding.GetEncoding(encoding);
        }
    }

    public virtual AggregationType AggregationType => AggregationType.Table;

    public virtual bool ExportAllRecords => false;

    protected abstract string DefaultOutputFileExt { get; }

    protected string OutputFileExt { get; }

    protected DataTargetBase()
    {
        OutputFileExt = DefaultOutputFileExt;
        var dataTargetAttr = GetType().GetCustomAttribute<DataTargetAttribute>();
        if (dataTargetAttr == null)
        {
            return;
        }

        var namespaze = dataTargetAttr.Name;
        var optionName = BuiltinOptionNames.OutputDataExtension;
        if (EnvManager.Current.TryGetOption(namespaze, optionName, false, out var optionExt))
        {
            if (!string.IsNullOrWhiteSpace(optionExt))
            {
                OutputFileExt = optionExt;
            }
        }
    }

    public abstract OutputFile ExportTable(DefTable table, List<Record> records);

    public virtual OutputFile ExportTables(List<DefTable> tables)
    {
        throw new NotSupportedException();
    }

    public virtual OutputFile ExportRecord(DefTable table, Record record)
    {
        throw new NotSupportedException();
    }

}
