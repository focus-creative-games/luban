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

﻿using System.Numerics;
using System.Xml;
using Luban.DataLoader;
using Luban.DataLoader.Builtin;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Xml;

public class XmlDataVisitor : IDataActionVisitor<XmlWriter>
{
    public static XmlDataVisitor Ins { get; } = new();

    public void Accept(DBool type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DByte type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DShort type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DInt type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DLong type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DFloat type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DDouble type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DEnum type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DString type, XmlWriter w)
    {
        w.WriteValue(type.Value);
    }

    public void Accept(DDateTime type, XmlWriter w)
    {
        w.WriteValue(type.UnixTimeOfCurrentContext());
    }

    public void Accept(DBean type, XmlWriter w)
    {
        if (type.Type.IsAbstractType)
        {
            w.WriteAttributeString(FieldNames.XmlTypeNameKey, DataUtil.GetImplTypeName(type));
        }

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
                w.WriteStartElement(defField.Name);
                d.Apply(this, w);
                w.WriteEndElement();
            }
        }
    }

    private void WriteList(List<DType> datas, XmlWriter w)
    {
        foreach (var d in datas)
        {
            w.WriteStartElement("ele");
            d.Apply(this, w);
            w.WriteEndElement();
        }
    }

    public void Accept(DArray type, XmlWriter w)
    {
        WriteList(type.Datas, w);
    }

    public void Accept(DList type, XmlWriter w)
    {
        WriteList(type.Datas, w);
    }

    public void Accept(DSet type, XmlWriter w)
    {
        WriteList(type.Datas, w);
    }

    public void Accept(DMap type, XmlWriter w)
    {
        foreach (var (k, v) in type.DataMap)
        {
            w.WriteStartElement("ele");
            w.WriteStartElement("key");
            k.Apply(this, w);
            w.WriteEndElement();
            w.WriteStartElement("value");
            v.Apply(this, w);
            w.WriteEndElement();
            w.WriteEndElement();
        }
    }
}
