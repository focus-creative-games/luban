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

﻿using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.FlatBuffers.TypeVisitors;

public class MapKeyValueEntryCollection
{
    public Dictionary<string, TMap> KeyValueEntries { get; } = new();

    public HashSet<string> VisitedTypes { get; } = new();
}

public class CollectMapKeyValueEntriesVisitor : DecoratorActionVisitor<MapKeyValueEntryCollection>
{
    public static CollectMapKeyValueEntriesVisitor Ins { get; } = new();


    public override void DoAccept(TType type, MapKeyValueEntryCollection x)
    {

    }

    public void Accept(DefBean bean, MapKeyValueEntryCollection x)
    {
        if (!x.VisitedTypes.Add(bean.FullName))
        {
            return;
        }
        if (bean.IsAbstractType)
        {
            foreach (var c in bean.HierarchyNotAbstractChildren)
            {
                Accept(c, x);
            }
        }
        else
        {
            foreach (var field in bean.HierarchyFields)
            {
                field.CType.Apply(this, x);
            }
        }
    }

    private static string MakeKeyValueType(TMap type)
    {
        return $"{type.KeyType.Apply(FlatBuffersTypeNameVisitor.Ins)}_{type.ValueType.Apply(FlatBuffersTypeNameVisitor.Ins)}";
    }

    public override void Accept(TBean type, MapKeyValueEntryCollection x)
    {
        Accept(type.DefBean, x);
    }

    public override void Accept(TArray type, MapKeyValueEntryCollection x)
    {
        if (type.ElementType is TBean tbean)
        {
            tbean.Apply(this, x);
        }
    }

    public override void Accept(TList type, MapKeyValueEntryCollection x)
    {
        if (type.ElementType is TBean tbean)
        {
            tbean.Apply(this, x);
        }
    }

    public override void Accept(TMap type, MapKeyValueEntryCollection x)
    {
        x.KeyValueEntries[MakeKeyValueType(type)] = type;
        if (type.ValueType is TBean tbean)
        {
            tbean.Apply(this, x);
        }
    }
}
