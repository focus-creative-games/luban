using System.Text;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Utils;
using MessagePack;

namespace Luban.MsgPack;

[DataTarget("msgpack")]
public class MsgPackDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "bytes";


    public void WriteList(DefTable table, List<Record> records, ref MessagePackWriter writer)
    {
        writer.WriteArrayHeader(records.Count);
        foreach (var record in records)
        {
            MsgPackDataVisitor.Ins.Accept(record.Data, ref writer);
        }
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var ms = new System.Buffers.ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(ms);
        WriteList(table, records, ref writer);
        writer.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = ms.WrittenSpan.ToArray(),
        };
    }
}
