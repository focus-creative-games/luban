using Google.Protobuf;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Protobuf.DataVisitors;
using Luban.Utils;

namespace Luban.Protobuf.DataTarget;

[DataTarget("protobuf2-bin")]
public class Protobuf2BinDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "bytes";

    public void WriteList(DefTable table, List<Record> datas, MemoryStream x)
    {
        var cos = new CodedOutputStream(x);
        foreach (var d in datas)
        {
            cos.WriteTag(1, WireFormat.WireType.LengthDelimited);
            d.Data.Apply(ProtobufBinDataVisitor.Ins, cos);
        }
        cos.Flush();
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var ss = new MemoryStream();
        WriteList(table, records, ss);
        ss.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
}
