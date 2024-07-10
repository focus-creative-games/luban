using System.Text;
using System.Text.Json;
using Luban.DataExporter.Builtin.Binary;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Json;

[DataTarget("json2")]
public class Json2DataTarget : JsonDataTarget
{
    protected override string DefaultOutputFileExt => "json";

    private void WriteAsObject(DefTable table, List<Record> datas, Utf8JsonWriter x)
    {
        switch (table.Mode)
        {
            case TableMode.ONE:
            {
                datas[0].Data.Apply(Json2DataVisitor.Ins, x);
                break;
            }
            case TableMode.MAP:
            {

                x.WriteStartObject();
                string indexName = table.IndexField.Name;
                foreach (var rec in datas)
                {
                    var indexFieldData = rec.Data.GetField(indexName);
                    x.WritePropertyName(indexFieldData.Apply(ToJsonPropertyNameVisitor.Ins));
                    rec.Data.Apply(Json2DataVisitor.Ins, x);
                }

                x.WriteEndObject();
                break;
            }
            case TableMode.LIST:
            {
                WriteAsArray(datas, x, Json2DataVisitor.Ins);
                break;
            }
            default:
            {
                throw new NotSupportedException($"not support table mode:{table.Mode}");
            }
        }
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
        WriteAsObject(table, records, jsonWriter);
        jsonWriter.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
}
