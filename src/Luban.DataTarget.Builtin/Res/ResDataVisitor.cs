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
using Luban.Types;

namespace Luban.DataExporter.Builtin.Res;

public class ResDataVisitor : IDataActionVisitor<TType, List<ResourceInfo>>
{
    public const string ResTagName = "res";

    public static ResDataVisitor Ins { get; } = new();

    public void Accept(DBool type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DByte type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DShort type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DInt type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DLong type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DFloat type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DDouble type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DEnum type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DString type, TType x, List<ResourceInfo> y)
    {
        if (!string.IsNullOrEmpty(type.Value) && x.HasTag(ResTagName))
        {
            y.Add(new ResourceInfo() { Resource = type.Value, Tag = x.GetTag(ResTagName) });
        }
    }

    public void Accept(DDateTime type, TType x, List<ResourceInfo> y)
    {

    }

    public void Accept(DBean type, TType x, List<ResourceInfo> y)
    {
        var def = type.ImplType;
        if (def == null)
        {
            return;
        }
        int index = 0;
        foreach (DType fieldData in type.Fields)
        {
            if (fieldData == null)
            {
                continue;
            }
            var fieldDef = ((DefField)def.HierarchyFields[index++]).CType;
            fieldData.Apply(this, fieldDef, y);
        }
    }

    private void Accept(List<DType> datas, TType elementType, List<ResourceInfo> ress)
    {
        foreach (var e in datas)
        {
            if (e != null)
            {
                e.Apply(this, elementType, ress);
            }
        }
    }

    public void Accept(DArray type, TType x, List<ResourceInfo> y)
    {
        Accept(type.Datas, type.Type.ElementType, y);
    }

    public void Accept(DList type, TType x, List<ResourceInfo> y)
    {
        Accept(type.Datas, type.Type.ElementType, y);
    }

    public void Accept(DSet type, TType x, List<ResourceInfo> y)
    {
        Accept(type.Datas, type.Type.ElementType, y);
    }

    public void Accept(DMap type, TType x, List<ResourceInfo> y)
    {
        TMap mtype = (TMap)x;
        foreach (var (k, v) in type.DataMap)
        {
            k.Apply(this, mtype.KeyType, y);
            v.Apply(this, mtype.ValueType, y);
        }
    }
}
