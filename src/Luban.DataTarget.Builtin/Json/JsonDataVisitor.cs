using System.Text.Json;
using Luban.DataLoader;
using Luban.DataLoader.Builtin;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Json;

public class JsonDataVisitor : IDataActionVisitor<Utf8JsonWriter>
{
    public static JsonDataVisitor Ins { get; } = new();

    public void Accept(DBool type, Utf8JsonWriter x)
    {
        x.WriteBooleanValue(type.Value);
    }

    public void Accept(DByte type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DShort type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DInt type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DLong type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DFloat type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DDouble type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public virtual void Accept(DEnum type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DString type, Utf8JsonWriter x)
    {
        x.WriteStringValue(type.Value);
    }

    public virtual void Accept(DDateTime type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.UnixTimeOfCurrentContext());
    }

    public virtual void Accept(DBean type, Utf8JsonWriter x)
    {
        x.WriteStartObject();

        if (type.Type.IsAbstractType)
        {
            x.WritePropertyName(FieldNames.JsonTypeNameKey);
            x.WriteStringValue(DataUtil.GetImplTypeName(type));
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

    public void WriteList(List<DType> datas, Utf8JsonWriter x)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Apply(this, x);
        }
        x.WriteEndArray();
    }

    public void Accept(DArray type, Utf8JsonWriter x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DList type, Utf8JsonWriter x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DSet type, Utf8JsonWriter x)
    {
        WriteList(type.Datas, x);
    }

    public virtual void Accept(DMap type, Utf8JsonWriter x)
    {
        x.WriteStartArray();
        foreach (var d in type.Datas)
        {
            x.WriteStartArray();
            d.Key.Apply(this, x);
            d.Value.Apply(this, x);
            x.WriteEndArray();
        }
        x.WriteEndArray();
    }
}
