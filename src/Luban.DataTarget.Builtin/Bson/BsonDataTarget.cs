using Luban.Core;
using Luban.Core.DataTarget;
using Luban.Core.Defs;
using Luban.Core.Utils;
using Newtonsoft.Json.Bson;

namespace Luban.DataExporter.Builtin.Bson;

[DataTarget("bson")]
public class BsonDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "json";
    

    private void WriteAsArray(List<Record> datas, BsonDataWriter x)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Data.Apply(BsonDataVisitor.Ins, x);
        }
        x.WriteEndArray();
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var ss = new MemoryStream();
        var bsonWriter = new BsonDataWriter(ss);
        WriteAsArray(records, bsonWriter);
        bsonWriter.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
}