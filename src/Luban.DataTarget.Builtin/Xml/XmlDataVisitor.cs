using System.Numerics;
using System.Xml;
using Luban.DataLoader;
using Luban.DataLoader.Builtin;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Xml;

public class XmlDataVisitor : IDataActionVisitor<XmlWriter>
{
    public static XmlDataVisitor Ins { get; } = new();

    public void Accept(DBool type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DByte type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DShort type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DInt type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DLong type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DFloat type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DDouble type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DEnum type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DString type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DDateTime type, XmlWriter w)
    {
        w.WriteValue(type.UnixTimeOfCurrentContext());
    }

    public void Accept(DBean type, XmlWriter w)
    {
        if (type.Type.IsAbstractType)
        {
            w.WriteAttributeString(FieldNames.XmlTypeNameKey, DataUtil.GetImplTypeName(type));
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
                w.WriteStartElement(defField.Name);
                d.Apply(this, w);
                w.WriteEndElement();
            }
        }
    }

    private void WriteList(List<DType> datas, XmlWriter w)
    {
        foreach (var d in datas)
        {
            w.WriteStartElement("ele");
            d.Apply(this, w);
            w.WriteEndElement();
        }
    }

    public void Accept(DArray type, XmlWriter w)
    {
        WriteList(type.Datas, w);
    }

    public void Accept(DList type, XmlWriter w)
    {
        WriteList(type.Datas, w);
    }

    public void Accept(DSet type, XmlWriter w)
    {
        WriteList(type.Datas, w);
    }

    public void Accept(DMap type, XmlWriter w)
    {
        foreach (var (k, v) in type.Datas)
        {
            w.WriteStartElement("ele");
            w.WriteStartElement("key");
            k.Apply(this, w);
            w.WriteEndElement();
            w.WriteStartElement("value");
            v.Apply(this, w);
            w.WriteEndElement();
            w.WriteEndElement();
        }
    }
}
