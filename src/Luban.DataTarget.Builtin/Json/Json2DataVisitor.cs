using System.Text.Json;
using Luban.Datas;

namespace Luban.DataExporter.Builtin.Json;

class Json2DataVisitor : JsonDataVisitor
{
    public static new Json2DataVisitor Ins { get; } = new();

    public override void Accept(DMap type, Utf8JsonWriter x)
    {
        x.WriteStartObject();
        foreach (var d in type.Datas)
        {
            x.WritePropertyName(d.Key.Apply(ToJsonPropertyNameVisitor.Ins));
            d.Value.Apply(this, x);
        }
        x.WriteEndObject();
    }
}
