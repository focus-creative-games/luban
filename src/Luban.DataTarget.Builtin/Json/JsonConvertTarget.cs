using System.Text.Json;
using Luban.Core;
using Luban.Core.DataTarget;
using Luban.Core.DataVisitors;
using Luban.Core.Defs;
using Luban.Core.Serialization;
using Luban.Core.Utils;
using Luban.DataExporter.Builtin.Binary;
using Luban.DataExporter.Builtin.FlatBuffers;

namespace Luban.DataExporter.Builtin.Json;

[DataTarget("json-convert")]
public class JsonConvertTarget : DataTargetBase
{
    protected override string OutputFileExt => "json";

    public static bool UseCompactJson => GenerationContext.Ins.GetBoolOptionOrDefault($"{FamilyPrefix}.json", "compact", true, false);
    
    protected virtual JsonDataVisitor ImplJsonDataVisitor => JsonConvertor.Ins;

    public void WriteAsArray(List<Record> datas, Utf8JsonWriter x, JsonDataVisitor jsonDataVisitor)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Data.Apply(jsonDataVisitor, x);
        }
        x.WriteEndArray();
    }

    public override OutputFile ExportRecord(DefTable table, Record record)
    {                  
        var ss = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
        {
            Indented = !UseCompactJson,
            SkipValidation = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });
        record.Data.Apply(JsonConvertor.Ins, jsonWriter);
        jsonWriter.Flush();
        var fileName = table.IsMapTable ?
            record.Data.GetField(table.IndexField.Name).Apply(ToStringVisitor2.Ins).Replace("\"", "").Replace("'", "")
            : record.AutoIndex.ToString();
        return new OutputFile()
        {
            File = $"{table.FullName}/{fileName}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
    
    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        throw new NotSupportedException();
    }
}