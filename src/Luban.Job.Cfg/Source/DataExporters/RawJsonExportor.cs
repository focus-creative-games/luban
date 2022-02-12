using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.DataExporters
{
    class RawJsonExportor : IDataActionVisitor<Utf8JsonWriter>
    {
        public static RawJsonExportor Ins { get; } = new();

        public void WriteAsArray(List<Record> datas, Utf8JsonWriter x)
        {
            x.WriteStartArray();
            foreach (var d in datas)
            {
                d.Data.Apply(this, x);
            }
            x.WriteEndArray();
        }

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
            x.WriteStringValue(type.StrValue);
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
            x.WriteStartObject();
            x.WritePropertyName(DText.KEY_NAME);
            x.WriteStringValue(type.Key);
            x.WritePropertyName(DText.TEXT_NAME);
            x.WriteStringValue(type.RawValue);
            x.WriteEndObject();
        }

        public void Accept(DBean type, Utf8JsonWriter x)
        {
            x.WriteStartObject();

            if (type.Type.IsAbstractType)
            {
                x.WritePropertyName(DefBean.JSON_TYPE_NAME_KEY);
                x.WriteStringValue(DataUtil.GetImplTypeName(type));
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
            x.WriteStringValue(DataUtil.FormatDateTime(type.Time));
        }
    }
}
