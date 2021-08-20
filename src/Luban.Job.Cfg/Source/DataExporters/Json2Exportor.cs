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
