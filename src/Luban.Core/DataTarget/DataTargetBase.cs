using Luban.Defs;

namespace Luban.DataTarget;

public abstract class DataTargetBase : IDataTarget
{
    public const string FamilyPrefix = "tableExporter";

    public virtual AggregationType AggregationType => AggregationType.Table;

    public virtual bool ExportAllRecords => false;

    protected abstract string OutputFileExt { get; }

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
