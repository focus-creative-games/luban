using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Luban.Job.Cfg.DataExporters
{
    public class XmlExportor : IDataActionVisitor<XmlWriter>
    {
        public static XmlExportor Ins { get; } = new XmlExportor();

        public void WriteAsArray(List<Record> datas, XmlWriter w)
        {
            w.WriteStartDocument();
            w.WriteStartElement("table");
            foreach (var d in datas)
            {
                w.WriteStartElement("record");
                d.Data.Apply(this, w);
                w.WriteEndElement();
            }
            w.WriteEndElement();
            w.WriteEndDocument();
        }

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

        public void Accept(DFshort type, XmlWriter w)
        {
            w.WriteValue(type.Value);
        }

        public void Accept(DInt type, XmlWriter w)
        {
            w.WriteValue(type.Value);
        }

        public void Accept(DFint type, XmlWriter w)
        {
            w.WriteValue(type.Value);
        }

        public void Accept(DLong type, XmlWriter w)
        {
            w.WriteValue(type.Value);
        }

        public void Accept(DFlong type, XmlWriter w)
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

        public void Accept(DText type, XmlWriter w)
        {
            w.WriteElementString(DText.KEY_NAME, type.Key);
            w.WriteElementString(DText.TEXT_NAME, type.TextOfCurrentAssembly);
        }

        public void Accept(DBytes type, XmlWriter w)
        {
            throw new NotSupportedException();
        }

        public void Accept(DVector2 type, XmlWriter w)
        {
            Vector2 v = type.Value;
            w.WriteElementString("x", v.X.ToString());
            w.WriteElementString("y", v.Y.ToString());
        }

        public void Accept(DVector3 type, XmlWriter w)
        {
            Vector3 v = type.Value;
            w.WriteElementString("x", v.X.ToString());
            w.WriteElementString("y", v.Y.ToString());
            w.WriteElementString("z", v.Z.ToString());
        }

        public void Accept(DVector4 type, XmlWriter w)
        {
            Vector4 v = type.Value;
            w.WriteElementString("x", v.X.ToString());
            w.WriteElementString("y", v.Y.ToString());
            w.WriteElementString("z", v.Z.ToString());
            w.WriteElementString("w", v.W.ToString());
        }

        public void Accept(DDateTime type, XmlWriter w)
        {
            w.WriteValue(type.UnixTimeOfCurrentAssembly);
        }

        public void Accept(DBean type, XmlWriter w)
        {
            if (type.Type.IsAbstractType)
            {
                w.WriteAttributeString(DefBean.XML_TYPE_NAME_KEY, DataUtil.GetImplTypeName(type));
            }

            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var d in type.Fields)
            {
                var defField = (DefField)defFields[index++];

                // 特殊处理 bean 多态类型
                // 另外，不生成  xxx:null 这样
                if (d == null || !defField.NeedExport)
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
            foreach (var (k,v) in type.Datas)
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
}
