using System.Text.Json;
using Luban.Core;
using Luban.Core.DataTarget;
using Luban.Core.Defs;
using Luban.Core.Serialization;
using Luban.Core.Utils;
using Luban.DataExporter.Builtin.Binary;
using Luban.DataExporter.Builtin.FlatBuffers;

namespace Luban.DataExporter.Builtin.Json;

[DataTarget("json")]
public class JsonDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "json";

    public static bool UseCompactJson => GenerationContext.Current.GetBoolOptionOrDefault($"{FamilyPrefix}.json", "compact", true, false);
    
    protected virtual JsonDataVisitor ImplJsonDataVisitor => JsonDataVisitor.Ins;

    public void WriteAsArray(List<Record> datas, Utf8JsonWriter x, JsonDataVisitor jsonDataVisitor)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Data.Apply(jsonDataVisitor, x);
        }
        x.WriteEndArray();
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {                  
        var ss = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
        {
            Indented = !UseCompactJson,
            SkipValidation = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });
        WriteAsArray(records, jsonWriter, ImplJsonDataVisitor);
        jsonWriter.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
}