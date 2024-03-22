using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Serialization;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Binary;

public class BinaryDataVisitor : IDataActionVisitor<ByteBuf>
{
    public static BinaryDataVisitor Ins { get; } = new();

    public void Accept(DBool type, ByteBuf x)
    {
        x.WriteBool(type.Value);
    }

    public void Accept(DByte type, ByteBuf x)
    {
        x.WriteByte(type.Value);
    }

    public void Accept(DShort type, ByteBuf x)
    {
        x.WriteShort(type.Value);
    }

    public void Accept(DInt type, ByteBuf x)
    {
        x.WriteInt(type.Value);
    }

    public void Accept(DLong type, ByteBuf x)
    {
        x.WriteLong(type.Value);
    }

    public void Accept(DFloat type, ByteBuf x)
    {
        x.WriteFloat(type.Value);
    }

    public void Accept(DDouble type, ByteBuf x)
    {
        x.WriteDouble(type.Value);
    }

    public void Accept(DEnum type, ByteBuf x)
    {
        x.WriteInt(type.Value);
    }

    public void Accept(DString type, ByteBuf x)
    {
        x.WriteString(type.Value);
    }

    public void Accept(DDateTime type, ByteBuf x)
    {
        x.WriteLong(type.UnixTimeOfCurrentContext());
    }

    public void Accept(DBean type, ByteBuf x)
    {
        var bean = type.Type;
        if (bean.IsAbstractType)
        {
            x.WriteInt(type.ImplType.Id);
        }

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var field in type.Fields)
        {
            var defField = (DefField)defFields[index++];
            if (!defField.NeedExport())
            {
                continue;
            }
            if (defField.CType.IsNullable)
            {
                if (field != null)
                {
                    x.WriteBool(true);
                    field.Apply(this, x);
                }
                else
                {
                    x.WriteBool(false);
                }
            }
            else
            {
                field.Apply(this, x);
            }
        }
    }

    public void WriteList(List<DType> datas, ByteBuf x)
    {
        x.WriteSize(datas.Count);
        foreach (var d in datas)
        {
            d.Apply(this, x);
        }
    }

    public void Accept(DArray type, ByteBuf x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DList type, ByteBuf x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DSet type, ByteBuf x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DMap type, ByteBuf x)
    {
        Dictionary<DType, DType> datas = type.Datas;
        x.WriteSize(datas.Count);
        foreach (var e in datas)
        {
            e.Key.Apply(this, x);
            e.Value.Apply(this, x);
        }
    }
}
