using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.DataVisitors
{
    class JsonExportor : IDataActionVisitor<DefAssembly, Utf8JsonWriter>
    {
        public static JsonExportor Ins { get; } = new JsonExportor();

        public void WriteList(List<Record> datas, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartArray();
            foreach (var d in datas)
            {
                d.Data.Apply(this, ass, x);
            }
            x.WriteEndArray();
        }

        public void Accept(DBool type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteBooleanValue(type.Value);
        }

        public void Accept(DByte type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DShort type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DFshort type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DInt type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DFint type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DLong type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DFlong type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DFloat type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DDouble type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DEnum type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.Value);
        }

        public void Accept(DString type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStringValue(type.Value);
        }

        public void Accept(DBytes type, DefAssembly ass, Utf8JsonWriter x)
        {
            throw new NotImplementedException();
        }

        public void Accept(DText type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStringValue(type.GetText(ass.ExportTextTable, ass.NotConvertTextSet));
        }

        public void Accept(DBean type, DefAssembly ass, Utf8JsonWriter x)
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
                    d.Apply(this, ass, x);
                }
            }
            x.WriteEndObject();
        }

        public void WriteList(List<DType> datas, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartArray();
            foreach (var d in datas)
            {
                d.Apply(this, ass, x);
            }
            x.WriteEndArray();
        }

        public void Accept(DArray type, DefAssembly ass, Utf8JsonWriter x)
        {
            WriteList(type.Datas, ass, x);
        }

        public void Accept(DList type, DefAssembly ass, Utf8JsonWriter x)
        {
            WriteList(type.Datas, ass, x);
        }

        public void Accept(DSet type, DefAssembly ass, Utf8JsonWriter x)
        {
            WriteList(type.Datas, ass, x);
        }

        public void Accept(DMap type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartArray();
            foreach (var d in type.Datas)
            {
                x.WriteStartArray();
                d.Key.Apply(this, ass, x);
                d.Value.Apply(this, ass, x);
                x.WriteEndArray();
            }
            x.WriteEndArray();
        }

        public void Accept(DVector2 type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WriteNumber("x", type.Value.X);
            x.WriteNumber("y", type.Value.Y);
            x.WriteEndObject();
        }

        public void Accept(DVector3 type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WriteNumber("x", type.Value.X);
            x.WriteNumber("y", type.Value.Y);
            x.WriteNumber("z", type.Value.Z);
            x.WriteEndObject();
        }

        public void Accept(DVector4 type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WriteNumber("x", type.Value.X);
            x.WriteNumber("y", type.Value.Y);
            x.WriteNumber("z", type.Value.Z);
            x.WriteNumber("w", type.Value.W);
            x.WriteEndObject();
        }

        public void Accept(DDateTime type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteNumberValue(type.GetUnixTime(ass.TimeZone));
        }
    }
}
