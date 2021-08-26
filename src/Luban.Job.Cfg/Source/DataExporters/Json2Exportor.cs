using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataExporters
{
    class Json2Exportor : JsonExportor
    {
        public new static Json2Exportor Ins { get; } = new();

        public void WriteAsObject(DefTable table, List<Record> datas, DefAssembly ass, Utf8JsonWriter x)
        {
            switch (table.Mode)
            {
                case RawDefs.ETableMode.ONE:
                {
                    this.Accept(datas[0].Data, ass, x);
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
                        this.Accept(rec.Data, ass, x);
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

        public override void Accept(DMap type, DefAssembly ass, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            foreach (var d in type.Datas)
            {
                x.WritePropertyName(d.Key.Apply(ToJsonPropertyNameVisitor.Ins));
                d.Value.Apply(this, ass, x);
            }
            x.WriteEndObject();
        }
    }
}
