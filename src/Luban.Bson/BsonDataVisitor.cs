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

using Luban.DataLoader;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;
using Newtonsoft.Json.Bson;

namespace Luban.Bson;

public class BsonDataVisitor : IDataActionVisitor<BsonDataWriter>
{
    public static BsonDataVisitor Ins { get; } = new();

    public void Accept(DBool type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DByte type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DShort type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DInt type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DLong type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DFloat type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DDouble type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public virtual void Accept(DEnum type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public void Accept(DString type, BsonDataWriter x)
    {
        x.WriteValue(type.Value);
    }

    public virtual void Accept(DDateTime type, BsonDataWriter x)
    {
        x.WriteValue(type.UnixTimeOfCurrentContext());
    }

    public virtual void Accept(DBean type, BsonDataWriter x)
    {
        x.WriteStartObject();

        if (type.Type.IsAbstractType)
        {
            x.WritePropertyName(FieldNames.JsonTypeNameKey);
            x.WriteValue(DataUtil.GetImplTypeName(type));
        }

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var d in type.Fields)
        {
            var defField = defFields[index++];

            // 特殊处理 bean 多态类型
            // 另外，不生成  xxx:null 这样
            if (d == null || !defField.NeedExport())
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

    public void WriteList(List<DType> datas, BsonDataWriter x)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Apply(this, x);
        }
        x.WriteEndArray();
    }

    public void Accept(DArray type, BsonDataWriter x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DList type, BsonDataWriter x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DSet type, BsonDataWriter x)
    {
        WriteList(type.Datas, x);
    }

    public virtual void Accept(DMap type, BsonDataWriter x)
    {
        x.WriteStartArray();
        foreach (var d in type.DataMap)
        {
            x.WriteStartArray();
            d.Key.Apply(this, x);
            d.Value.Apply(this, x);
            x.WriteEndArray();
        }
        x.WriteEndArray();
    }
}
