using System.Text.Json;
using Luban.DataLoader;
using Luban.DataLoader.Builtin;
using Luban.Datas;
using Luban.Defs;

namespace Luban.DataExporter.Builtin.Json;

public class JsonConvertor : JsonDataVisitor
{
    public static new JsonConvertor Ins { get; } = new();

    public override void Accept(DEnum type, Utf8JsonWriter x)
    {
        x.WriteStringValue(type.StrValue);
    }

    public override void Accept(DBean type, Utf8JsonWriter x)
    {
        x.WriteStartObject();

        if (type.Type.IsAbstractType)
        {
            x.WritePropertyName(FieldNames.JsonTypeNameKey);
            x.WriteStringValue(type.ImplType.Name);
        }

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var d in type.Fields)
        {
            var defField = (DefField)defFields[index++];

            // 特殊处理 bean 多态类型
            // 另外，不生成  xxx:null 这样
            if (d == null)
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


    public override void Accept(DDateTime type, Utf8JsonWriter x)
    {
        x.WriteStringValue(type.ToFormatString());
    }
}
