// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
        return CreateOutputFile($"{table.OutputDataFile}.{OutputFileExt}", Encoding.UTF8.GetString(DataUtil.StreamToBytes(ss)));
    }
}
