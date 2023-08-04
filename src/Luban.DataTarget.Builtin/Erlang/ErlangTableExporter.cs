using System.Text;
using Luban.Core;
using Luban.Core.Datas;
using Luban.Core.DataTarget;
using Luban.Core.Defs;

namespace Luban.DataExporter.Builtin.Erlang;

[DataTarget("erlang")]
public class ErlangExport : DataTargetBase
{
    public static ErlangExport Ins { get; } = new();

    public void ExportTableSingleton(DefTable t, Record record, StringBuilder s)
    {
        s.Append($"-module({t.FullName.Replace('.', '_').ToLower()}).").AppendLine();
        s.Append($"-export([get_data/0]).").AppendLine();


        s.Append("get_data() -> ").AppendLine();
        s.Append('\t').Append(record.Data.Apply(ToErlangLiteralVisitor.Ins));
        s.Append('.');
    }

    public void ExportTableMap(DefTable t, List<Record> records, StringBuilder s)
    {
        s.Append($"-module({t.FullName.Replace('.', '_').ToLower()}).").AppendLine();
        s.Append($"-export([get_data_map/0, get_key_list/0]).").AppendLine();

        s.Append("get_data_map() -> #{").AppendLine();

        int index = 0;
        foreach (Record r in records)
        {
            if (index++ > 0)
            {
                s.Append(',').AppendLine();
            }
            DBean d = r.Data;
            s.Append($"\t{d.GetField(t.Index).Apply(ToErlangLiteralVisitor.Ins)} => ");
            s.Append(d.Apply(ToErlangLiteralVisitor.Ins));
        }
        s.Append("}.").AppendLine();

        s.Append($"get_key_list() ->").AppendLine();
        s.Append($"\t[{string.Join(',', records.Select(r => r.Data.GetField(t.Index).Apply(ToErlangLiteralVisitor.Ins)))}].");
    }

    protected override string OutputFileExt => "erl";
    
    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var s = new StringBuilder();
        if (table.IsMapTable)
        {
            ExportTableMap(table, records, s);
        }
        else
        {
            ExportTableSingleton(table, records[0], s);
        }
        return new OutputFile() 
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = s.ToString(),
        };
    }
}