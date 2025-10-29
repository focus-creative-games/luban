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

﻿using Luban.DataLoader;
using Luban.Datas;
using Luban.Defs;
using Luban.Utils;
using MessagePack;

namespace Luban.MsgPack;

class MsgPackDataVisitor
{
    public static MsgPackDataVisitor Ins { get; } = new();

    public void Apply(DType type, ref MessagePackWriter writer)
    {
        switch (type)
        {
            case DInt x:
                Accept(x, ref writer);
                break;
            case DString x:
                Accept(x, ref writer);
                break;
            case DFloat x:
                Accept(x, ref writer);
                break;
            case DBean x:
                Accept(x, ref writer);
                break;
            case DBool x:
                Accept(x, ref writer);
                break;
            case DEnum x:
                Accept(x, ref writer);
                break;
            case DList x:
                Accept(x, ref writer);
                break;
            case DArray x:
                Accept(x, ref writer);
                break;
            case DLong x:
                Accept(x, ref writer);
                break;
            case DDateTime x:
                Accept(x, ref writer);
                break;
            case DMap x:
                Accept(x, ref writer);
                break;
            case DByte x:
                Accept(x, ref writer);
                break;
            case DDouble x:
                Accept(x, ref writer);
                break;
            case DSet x:
                Accept(x, ref writer);
                break;
            case DShort x:
                Accept(x, ref writer);
                break;
            default:
                throw new NotSupportedException($"DType:{type.GetType().FullName} not support");
        }
    }

    public void Accept(DBool type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DByte type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DShort type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DInt type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DLong type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DFloat type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DDouble type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DEnum type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DString type, ref MessagePackWriter writer)
    {
        writer.Write(type.Value);
    }

    public void Accept(DDateTime type, ref MessagePackWriter writer)
    {
        writer.Write(type.UnixTimeOfCurrentContext());
    }

    public void Accept(DBean type, ref MessagePackWriter writer)
    {
        var implType = type.ImplType;
        var hierarchyFields = implType.HierarchyFields;
        int exportCount = 0;
        {
            if (type.Type.IsAbstractType)
            {
                exportCount++;
            }
            int idx = 0;
            foreach (var field in type.Fields)
            {
                var defField = (DefField)hierarchyFields[idx++];
                if (field == null || !defField.NeedExport())
                {
                    continue;
                }
                ++exportCount;
            }
        }

        writer.WriteMapHeader(exportCount);
        if (type.Type.IsAbstractType)
        {
            writer.Write(FieldNames.JsonTypeNameKey);
            writer.Write(DataUtil.GetImplTypeName(type));
        }

        int index = 0;
        foreach (var field in type.Fields)
        {
            var defField = (DefField)hierarchyFields[index++];
            if (field == null || !defField.NeedExport())
            {
                continue;
            }
            writer.Write(defField.Name);
            Apply(field, ref writer);
        }
    }

    public void Accept(DArray type, ref MessagePackWriter writer)
    {
        writer.WriteArrayHeader(type.Datas.Count);
        foreach (var d in type.Datas)
        {
            Apply(d, ref writer);
        }
    }

    public void Accept(DList type, ref MessagePackWriter writer)
    {
        writer.WriteArrayHeader(type.Datas.Count);
        foreach (var d in type.Datas)
        {
            Apply(d, ref writer);
        }
    }

    public void Accept(DSet type, ref MessagePackWriter writer)
    {
        writer.WriteArrayHeader(type.Datas.Count);
        foreach (var d in type.Datas)
        {
            Apply(d, ref writer);
        }
    }

    public void Accept(DMap type, ref MessagePackWriter writer)
    {
        writer.WriteMapHeader(type.DataMap.Count);
        foreach (var d in type.DataMap)
        {
            Apply(d.Key, ref writer);
            Apply(d.Value, ref writer);
        }
    }
}
