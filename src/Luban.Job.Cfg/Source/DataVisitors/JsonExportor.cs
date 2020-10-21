using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.DataVisitors
{
    class JsonExportor : IDataActionVisitor<Utf8JsonWriter>
    {
        public static JsonExportor Ins { get; } = new JsonExportor();

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

        public void Accept(DFshort type, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DInt type, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DFint type, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DLong type, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DFlong type, Utf8JsonWriter x)
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

        public void Accept(DEnum type, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DString type, Utf8JsonWriter x)
        {
            x.WriteStringValue(type.Value);
        }

        public void Accept(DBytes type, Utf8JsonWriter x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DText type, Utf8JsonWriter x)
        {
            x.WriteStringValue(type.Value);
        }

        public void Accept(DBean type, Utf8JsonWriter x)
        {
            x.WriteStartObject();

            if (type.Type.IsAbstractType)
            {
                x.WritePropertyName(DefBean.TYPE_NAME_KEY);
                x.WriteStringValue(type.ImplType.Name);
            }

            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var d in type.Fields)
            {
                var defField = (DefField)defFields[index++];
                if (!defField.NeedExport)
                {
                    continue;
                }

                x.WritePropertyName(defField.Name);
                // 特殊处理 bean 多态类型
                if (d == null || (d is DBean db && db.ImplType == null))
                {
                    x.WriteNullValue();
                }
                else
                {
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

        public void Accept(DMap type, Utf8JsonWriter x)
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

        public void Accept(DVector2 type, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WriteNumber("x", type.Value.X);
            x.WriteNumber("y", type.Value.Y);
            x.WriteEndObject();
        }

        public void Accept(DVector3 type, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WriteNumber("x", type.Value.X);
            x.WriteNumber("y", type.Value.Y);
            x.WriteNumber("z", type.Value.Z);
            x.WriteEndObject();
        }

        public void Accept(DVector4 type, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WriteNumber("x", type.Value.X);
            x.WriteNumber("y", type.Value.Y);
            x.WriteNumber("z", type.Value.Z);
            x.WriteNumber("w", type.Value.W);
            x.WriteEndObject();
        }

        public void Accept(DDateTime type, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.UnixTime);
        }
    }
}
