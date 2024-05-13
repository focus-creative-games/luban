using System.Text.Json;
using Luban.DataExporter.Builtin.Json;
using Luban.DataTarget;
using Luban.Defs;
using Luban.FlatBuffers.DataVisitors;
using Luban.Utils;

namespace Luban.FlatBuffers.DataTarget;

[DataTarget("flatbuffers-json")]
public class FlatBuffersDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "json";

    private void WriteAsTable(List<Record> datas, Utf8JsonWriter x)
    {
        x.WriteStartObject();
        // 如果修改了这个名字，请同时修改table.tpl
        x.WritePropertyName("data_list");
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Data.Apply(FlatBuffersJsonDataVisitor.Ins, x);
        }
        x.WriteEndArray();
        x.WriteEndObject();
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var ss = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
        {
            Indented = !JsonDataTarget.UseCompactJson,
            SkipValidation = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });
        WriteAsTable(records, jsonWriter);
        jsonWriter.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
}
