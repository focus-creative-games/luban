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

using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Serialization;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Binary;

public class BinaryDataVisitor : IDataActionVisitor<ByteBuf>
{
    public static BinaryDataVisitor Ins { get; } = new();

    public void Accept(DBool type, ByteBuf x)
    {
        x.WriteBool(type.Value);
    }

    public void Accept(DByte type, ByteBuf x)
    {
        x.WriteByte(type.Value);
    }

    public void Accept(DShort type, ByteBuf x)
    {
        x.WriteShort(type.Value);
    }

    public void Accept(DInt type, ByteBuf x)
    {
        x.WriteInt(type.Value);
    }

    public void Accept(DLong type, ByteBuf x)
    {
        x.WriteLong(type.Value);
    }

    public void Accept(DFloat type, ByteBuf x)
    {
        x.WriteFloat(type.Value);
    }

    public void Accept(DDouble type, ByteBuf x)
    {
        x.WriteDouble(type.Value);
    }

    public void Accept(DEnum type, ByteBuf x)
    {
        x.WriteInt(type.Value);
    }

    public void Accept(DString type, ByteBuf x)
    {
        x.WriteString(type.Value);
    }

    public void Accept(DDateTime type, ByteBuf x)
    {
        x.WriteLong(type.UnixTimeOfCurrentContext());
    }

    public void Accept(DBean type, ByteBuf x)
    {
        var bean = type.Type;
        if (bean.IsAbstractType)
        {
            x.WriteInt(type.ImplType.Id);
        }

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var field in type.Fields)
        {
            var defField = (DefField)defFields[index++];
            if (!defField.NeedExport())
            {
                continue;
            }
            if (defField.CType.IsNullable)
            {
                if (field != null)
                {
                    x.WriteBool(true);
                    field.Apply(this, x);
                }
                else
                {
                    x.WriteBool(false);
                }
            }
            else
            {
                field.Apply(this, x);
            }
        }
    }

    public void WriteList(List<DType> datas, ByteBuf x)
    {
        x.WriteSize(datas.Count);
        foreach (var d in datas)
        {
            d.Apply(this, x);
        }
    }

    public void Accept(DArray type, ByteBuf x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DList type, ByteBuf x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DSet type, ByteBuf x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DMap type, ByteBuf x)
    {
        Dictionary<DType, DType> datas = type.DataMap;
        x.WriteSize(datas.Count);
        foreach (var e in datas)
        {
            e.Key.Apply(this, x);
            e.Value.Apply(this, x);
        }
    }
}
