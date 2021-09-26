﻿using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.DataExporters
{
    class Json2Exportor : JsonExportor
    {
        public new static Json2Exportor Ins { get; } = new();

        public void WriteAsObject(DefTable table, List<Record> datas, Utf8JsonWriter x)
        {
            switch (table.Mode)
            {
                case RawDefs.ETableMode.ONE:
                {
                    this.Accept(datas[0].Data, x);
                    break;
                }
                case RawDefs.ETableMode.MAP:
                {

                    x.WriteStartObject();
                    string indexName = table.IndexField.Name;
                    foreach (var rec in datas)
                    {
                        var indexFieldData = rec.Data.GetField(indexName);

                        x.WritePropertyName(indexFieldData.Apply(ToJsonPropertyNameVisitor.Ins));
                        this.Accept(rec.Data, x);
                    }

                    x.WriteEndObject();
                    break;
                }
                default:
                {
                    throw new NotSupportedException($"not support table mode:{table.Mode}");
                }
            }
        }

        public override void Accept(DMap type, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            foreach (var d in type.Datas)
            {
                x.WritePropertyName(d.Key.Apply(ToJsonPropertyNameVisitor.Ins));
                d.Value.Apply(this, x);
            }
            x.WriteEndObject();
        }
    }
}
