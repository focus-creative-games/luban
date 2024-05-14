using System.Text.Json;
using Luban.DataTarget;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Json;

[DataTarget("json-convert")]
public class JsonConvertTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "json";

    public static bool UseCompactJson => EnvManager.Current.GetBoolOptionOrDefault("json", "compact", true, false);

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
