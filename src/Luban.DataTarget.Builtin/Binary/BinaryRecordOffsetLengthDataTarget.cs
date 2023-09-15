using Luban.Datas;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Serialization;
using YamlDotNet.Core.Tokens;

namespace Luban.DataExporter.Builtin.Binary;

[DataTarget("bin-offsetlength")]
public class BinaryRecordOffsetLengthDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "bytes";

    private void WriteList(DefTable table, List<Record> datas, ByteBuf x)
    {
        ByteBuf buf = new ByteBuf(10 * 1024);
        x.WriteSize(datas.Count);
        buf.WriteSize(datas.Count);
        int lastOffset = 0;
        foreach (var d in datas)
        {
            foreach (var indexInfo in table.IndexList)
            {
                DType keyData = d.Data.Fields[indexInfo.IndexFieldIdIndex];
                keyData.Apply(BinaryDataVisitor.Ins, x);
            }
            int offset = buf.Size;
            x.WriteSize(offset);
            d.Data.Apply(BinaryDataVisitor.Ins, buf);
            int length = buf.Size - lastOffset;
            x.WriteSize(length);
            lastOffset = buf.Size;
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