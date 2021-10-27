using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luban.Job.Cfg.DataExporters
{
    class ErlangExport
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
    }
}
