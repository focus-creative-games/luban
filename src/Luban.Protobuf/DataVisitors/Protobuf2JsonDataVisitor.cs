using System.Text.Json;
using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Defs;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Protobuf.DataVisitors;

public class Protobuf2JsonDataVisitor : JsonDataVisitor
{
    public static new Protobuf2JsonDataVisitor Ins { get; } = new();

    public override void Accept(DBean type, Utf8JsonWriter x)
    {
        x.WriteStartObject();

        if (type.Type.IsAbstractType)
        {
            x.WritePropertyName(type.ImplType.Name);
            x.WriteStartObject();
        }

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var d in type.Fields)
        {
            var defField = (DefField)defFields[index++];

            // 特殊处理 bean 多态类型
            // 另外，不生成  xxx:null 这样
            if (d == null || !defField.NeedExport())
            {
                //x.WriteNullValue();
            }
            else
            {
                x.WritePropertyName(defField.Name);
                d.Apply(this, x);
            }
        }
        if (type.Type.IsAbstractType)
        {
            x.WriteEndObject();
        }
        x.WriteEndObject();
    }


    public override void Accept(DMap type, Utf8JsonWriter x)
    {
        x.WriteStartObject();
        foreach (var d in type.DataMap)
        {
            x.WritePropertyName(d.Key.Apply(ToJsonLiteralVisitor.Ins));
            d.Value.Apply(this, x);
        }
        x.WriteEndObject();
    }
}
