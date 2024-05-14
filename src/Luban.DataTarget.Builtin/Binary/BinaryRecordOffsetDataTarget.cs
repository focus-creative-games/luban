using Luban.Datas;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Serialization;

namespace Luban.DataExporter.Builtin.Binary;

[DataTarget("bin-offset")]
public class BinaryRecordOffsetDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "bytes";

    private void WriteList(DefTable table, List<Record> datas, ByteBuf x)
    {
        ByteBuf buf = new ByteBuf();
        buf.WriteSize(datas.Count);
        x.WriteSize(datas.Count);
        foreach (var d in datas)
        {
            foreach (var indexInfo in table.IndexList)
            {
                DType keyData = d.Data.Fields[indexInfo.IndexFieldIdIndex];
                keyData.Apply(BinaryDataVisitor.Ins, x);
            }
            x.WriteSize(buf.Size);
            d.Data.Apply(BinaryDataVisitor.Ins, buf);
        }
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var bytes = new ByteBuf();
        WriteList(table, records, bytes);
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = bytes.CopyData(),
        };
    }
}
