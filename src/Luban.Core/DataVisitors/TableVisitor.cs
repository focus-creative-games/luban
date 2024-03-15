using Luban.Datas;
using Luban.Defs;
using Luban.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataVisitors;

public class TableVisitor
{
    public static TableVisitor Ins { get; } = new();

    public void Visit<T>(DefTable table, IDataActionVisitor<T> visitor, T arg)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, arg);
    }

    public void Visit<T>(DefTable table, IDataActionVisitor2<T> visitor, T arg)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, arg);
    }

    public void Visit<T1, T2>(DefTable table, IDataActionVisitor<T1, T2> visitor, T1 a1, T2 a2)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, a1, a2);
    }

    public void Visit<T1, T2>(DefTable table, IDataActionVisitor2<T1, T2> visitor, T1 a1, T2 a2)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, a1, a2);
    }

    public void Visit<T>(DefTable table, List<Record> records, IDataActionVisitor<T> visitor, T arg)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, arg);
        }
    }

    public void Visit<T>(DefTable table, List<Record> records, IDataActionVisitor2<T> visitor, T arg)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, table.ValueTType, arg);
        }
    }

    public void Visit<T1, T2>(DefTable table, List<Record> records, IDataActionVisitor<T1, T2> visitor, T1 a1, T2 a2)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, a1, a2);
        }
    }

    public void Visit<T1, T2>(DefTable table, List<Record> records, IDataActionVisitor2<T1, T2> visitor, T1 a1, T2 a2)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, table.ValueTType, a1, a2);
        }
    }
}
