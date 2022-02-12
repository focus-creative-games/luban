using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataConverts
{
    class JsonConvertor : DataExporters.JsonExportor
    {
        public static new JsonConvertor Ins { get; } = new();

        public override void Accept(DText type, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            x.WritePropertyName(DText.KEY_NAME);
            x.WriteStringValue(type.Key);
            x.WritePropertyName(DText.TEXT_NAME);
            x.WriteStringValue(type.RawValue);
            x.WriteEndObject();
        }

        public override void Accept(DEnum type, Utf8JsonWriter x)
        {
            x.WriteStringValue(type.StrValue);
        }

        public override void Accept(DBean type, Utf8JsonWriter x)
        {
            x.WriteStartObject();

            if (type.Type.IsAbstractType)
            {
                x.WritePropertyName(DefBean.JSON_TYPE_NAME_KEY);
                x.WriteStringValue(type.ImplType.Name);
            }

            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var d in type.Fields)
            {
                var defField = (DefField)defFields[index++];

                // 特殊处理 bean 多态类型
                // 另外，不生成  xxx:null 这样
                if (d == null)
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


        public override void Accept(DDateTime type, Utf8JsonWriter x)
        {
            x.WriteStringValue(type.ToFormatString());
        }
    }
}
