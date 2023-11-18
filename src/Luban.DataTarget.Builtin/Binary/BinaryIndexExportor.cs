using Luban.Datas;
using Luban.Defs;
using Luban.Serialization;

namespace Luban.DataExporter.Builtin.Binary;

class BinaryIndexExportor
{
    public static BinaryIndexExportor Ins { get; } = new();

    public void WriteList(DefTable table, List<Record> datas, ByteBuf x)
    {
        x.WriteSize(datas.Count);
        var tableDataBuf = new ByteBuf(10 * 1024);
        tableDataBuf.WriteSize(datas.Count);

        foreach (var d in datas)
        {
            int offset = tableDataBuf.Size;
            d.Data.Apply(BinaryDataVisitor.Ins, tableDataBuf);

            string keyStr = "";
            foreach (IndexInfo index in table.IndexList)
            {
                DType key = d.Data.Fields[index.IndexFieldIdIndex];
                key.Apply(BinaryDataVisitor.Ins, x);
                keyStr += key.ToString() + ",";
            }
            x.WriteSize(offset);
            Console.WriteLine($"table:{table.Name} key:{keyStr} offset:{offset}");
        }

    }
}
