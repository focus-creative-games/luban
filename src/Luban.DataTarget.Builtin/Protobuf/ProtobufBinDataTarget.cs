using Google.Protobuf;
using Luban.Core;
using Luban.Core.DataTarget;
using Luban.Core.Defs;
using Luban.Core.Serialization;
using Luban.Core.Utils;
using Luban.DataExporter.Builtin.Binary;

namespace Luban.DataExporter.Builtin.Protobuf;

[DataTarget("protobuf-bin")]
public class ProtobufBinDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "bytes";
    
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