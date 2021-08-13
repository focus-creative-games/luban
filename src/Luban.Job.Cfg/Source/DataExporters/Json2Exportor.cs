using Luban.Job.Cfg.Datas;
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
            var keyType = type.Type.KeyType;
            if (keyType is not TString
                && keyType is not TInt
                && keyType is not TLong)
            {
                throw new Exception($"data_json2格式只支持key为int,long,string类型的map");
            }
            x.WriteStartObject();
            foreach (var d in type.Datas)
            {
                switch (d.Key)
                {
                    case DString ds:
                    {
                        x.WritePropertyName(ds.Value);
                        break;
                    }
                    case DInt di:
                    {
                        x.WritePropertyName(di.Value.ToString());
                        break;
                    }
                    case DLong dl:
                    {
                        x.WritePropertyName(dl.Value.ToString());
                        break;
                    }
                    default:
                    {
                        throw new Exception($"data_json2格式只支持key为int,long,string类型的map");
                    }
                }
                d.Value.Apply(this, ass, x);
            }
            x.WriteEndObject();
        }
    }
}
