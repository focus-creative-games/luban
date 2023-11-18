using System.Text.Json;
using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Defs;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Protobuf.DataVisitors;

public class ProtobufJsonDataVisitor : JsonDataVisitor
{
    public static new ProtobufJsonDataVisitor Ins { get; } = new();

    public override void Accept(DBean type, Utf8JsonWriter x)
    {
        x.WriteStartObject();

        if (type.Type.IsAbstractType)
        {
            // protobuf oneof 用 @type来识别类型
            x.WritePropertyName("@type");
            x.WriteStringValue(TBean.Create(false, type.ImplType, null).Apply(ProtobufTypeNameVisitor.Ins));
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
        x.WriteEndObject();
    }


    public override void Accept(DMap type, Utf8JsonWriter x)
    {
        x.WriteStartArray();
        foreach (var d in type.Datas)
        {
            x.WriteStartArray();
            x.WriteStringValue(d.Key.Apply(ToJsonLiteralVisitor.Ins));
            d.Value.Apply(this, x);
            x.WriteEndArray();
        }
        x.WriteEndArray();
    }
}
