// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text.Json;
using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Defs;
using Luban.FlatBuffers.TypeVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.FlatBuffers.DataVisitors;

public class FlatBuffersJsonDataVisitor : JsonDataVisitor
{
    public static new FlatBuffersJsonDataVisitor Ins { get; } = new();

    public override void Accept(DBean type, Utf8JsonWriter x)
    {
        x.WriteStartObject();

        // flatc 不允许有多余字段
        //if (type.Type.IsAbstractType)
        //{
        //    x.WritePropertyName(FieldNames.TYPE_NAME_KEY);
        //    x.WriteStringValue(type.ImplType.Name);
        //}

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var d in type.Fields)
        {
            var defField = (DefField)defFields[index++];

            // 特殊处理 bean 多态类型
            // 另外，不生成  xxx:null 这样
            if (d == null || !defField.NeedExport())
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

    public override void Accept(DMap type, Utf8JsonWriter x)
    {
        x.WriteStartArray();
        foreach (var d in type.DataMap)
        {
            x.WriteStartObject();
            x.WritePropertyName("key");
            d.Key.Apply(this, x);
            x.WritePropertyName("value");
            d.Value.Apply(this, x);
            x.WriteEndObject();
        }
        x.WriteEndArray();
    }
}
