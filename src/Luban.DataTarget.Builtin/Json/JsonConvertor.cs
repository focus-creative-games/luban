using System.Text.Json;
using Luban.Core.Datas;
using Luban.Core.Defs;
using Luban.DataLoader.Builtin;

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
            x.WritePropertyName(FieldNames.JSON_TYPE_NAME_KEY);
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