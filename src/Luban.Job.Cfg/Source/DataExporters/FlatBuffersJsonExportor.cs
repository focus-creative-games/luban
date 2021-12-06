using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.DataExporters
{
    class FlatBuffersJsonExportor : JsonExportor
    {
        public static new FlatBuffersJsonExportor Ins { get; } = new();

        public void WriteAsTable(List<Record> datas, Utf8JsonWriter x)
        {
            x.WriteStartObject();
            // 如果修改了这个名字，请同时修改table.tpl
            x.WritePropertyName("data_list");
            x.WriteStartArray();
            foreach (var d in datas)
            {
                d.Data.Apply(this, x);
            }
            x.WriteEndArray();
            x.WriteEndObject();
        }

        public override void Accept(DText type, Utf8JsonWriter x)
        {
            // 不支持本地化。只能简单起见这么做了
            //x.WriteStartObject();
            //x.WritePropertyName(DText.KEY_NAME);
            //x.WriteStringValue(type.Key);
            //x.WritePropertyName(DText.TEXT_NAME);
            x.WriteStringValue(type.TextOfCurrentAssembly);
            //x.WriteEndObject();
        }

        public override void Accept(DBean type, Utf8JsonWriter x)
        {
            x.WriteStartObject();

            // flatc 不允许有多余字段
            //if (type.Type.IsAbstractType)
            //{
            //    x.WritePropertyName(DefBean.TYPE_NAME_KEY);
            //    x.WriteStringValue(type.ImplType.Name);
            //}

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
                    // flatbuffers的union类型的json格式,会额外产生一个 xx_type字段。
                    // 另外，json格式不支持union出现在容器类型上。
                    if (d is DBean beanField && beanField.Type.IsAbstractType)
                    {
                        x.WritePropertyName($"{defField.Name}_type");
                        x.WriteStringValue(TBean.Create(defField.CType.IsNullable, beanField.ImplType, null).Apply(FlatBuffersTypeNameVisitor.Ins));
                    }

                    x.WritePropertyName(defField.Name);
                    d.Apply(this, x);
                }
            }
            x.WriteEndObject();
        }
    }
}
