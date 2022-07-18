using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.DataExporters
{
    class BsonExportor : IDataActionVisitor<BsonDataWriter>
    {
        public static BsonExportor Ins { get; } = new BsonExportor();

        public void WriteAsArray(List<Record> datas, BsonDataWriter x)
        {
            x.WriteStartArray();
            foreach (var d in datas)
            {
                d.Data.Apply(this, x);
            }
            x.WriteEndArray();
        }

        public void Accept(DBool type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DByte type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DShort type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DFshort type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DInt type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DFint type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DLong type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DFlong type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DFloat type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DDouble type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public virtual void Accept(DEnum type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DString type, BsonDataWriter x)
        {
            x.WriteValue(type.Value);
        }

        public void Accept(DBytes type, BsonDataWriter x)
        {
            throw new NotImplementedException();
        }

        public virtual void Accept(DText type, BsonDataWriter x)
        {
            x.WriteStartObject();
            x.WritePropertyName(DText.KEY_NAME);
            x.WriteValue(type.Key);
            x.WritePropertyName(DText.TEXT_NAME);
            x.WriteValue(type.TextOfCurrentAssembly);
            x.WriteEndObject();
        }

        public virtual void Accept(DBean type, BsonDataWriter x)
        {
            x.WriteStartObject();

            if (type.Type.IsAbstractType)
            {
                x.WritePropertyName(DefBean.JSON_TYPE_NAME_KEY);
                x.WriteValue(DataUtil.GetImplTypeName(type));
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
                    x.WritePropertyName(defField.Name);
                    d.Apply(this, x);
                }
            }
            x.WriteEndObject();
        }

        public void WriteList(List<DType> datas, BsonDataWriter x)
        {
            x.WriteStartArray();
            foreach (var d in datas)
            {
                d.Apply(this, x);
            }
            x.WriteEndArray();
        }

        public void Accept(DArray type, BsonDataWriter x)
        {
            WriteList(type.Datas, x);
        }

        public void Accept(DList type, BsonDataWriter x)
        {
            WriteList(type.Datas, x);
        }

        public void Accept(DSet type, BsonDataWriter x)
        {
            WriteList(type.Datas, x);
        }

        public virtual void Accept(DMap type, BsonDataWriter x)
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

        public void Accept(DVector2 type, BsonDataWriter x)
        {
            x.WriteStartObject();
            x.WritePropertyName("x"); x.WriteValue(type.Value.X);
            x.WritePropertyName("y"); x.WriteValue(type.Value.Y);
            x.WriteEndObject();
        }

        public void Accept(DVector3 type, BsonDataWriter x)
        {
            x.WriteStartObject();
            x.WritePropertyName("x"); x.WriteValue(type.Value.X);
            x.WritePropertyName("y"); x.WriteValue(type.Value.Y);
            x.WritePropertyName("z"); x.WriteValue(type.Value.Z);
            x.WriteEndObject();
        }

        public void Accept(DVector4 type, BsonDataWriter x)
        {
            x.WriteStartObject();
            x.WritePropertyName("x"); x.WriteValue(type.Value.X);
            x.WritePropertyName("y"); x.WriteValue(type.Value.Y);
            x.WritePropertyName("z"); x.WriteValue(type.Value.Z);
            x.WritePropertyName("w"); x.WriteValue(type.Value.W);
            x.WriteEndObject();
        }

        public virtual void Accept(DDateTime type, BsonDataWriter x)
        {
            x.WriteValue(type.UnixTimeOfCurrentAssembly);
        }
    }
}
