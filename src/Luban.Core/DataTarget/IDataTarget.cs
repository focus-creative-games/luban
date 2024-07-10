using Luban.Defs;
using System.Text;

namespace Luban.DataTarget;

public enum AggregationType
{
    Table,
    Tables,
    Record,
    Other,
}

public interface IDataTarget
{
    AggregationType AggregationType { get; }

    Encoding FileEncoding { get; }

    bool ExportAllRecords { get; }

    OutputFile ExportTable(DefTable table, List<Record> records);

    OutputFile ExportTables(List<DefTable> tables);

    OutputFile ExportRecord(DefTable table, Record record);
}
