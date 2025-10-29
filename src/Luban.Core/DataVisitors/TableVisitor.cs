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

﻿using Luban.Datas;
using Luban.Defs;
using Luban.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataVisitors;

public class TableVisitor
{
    public static TableVisitor Ins { get; } = new();

    public void Visit<T>(DefTable table, IDataActionVisitor<T> visitor, T arg)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, arg);
    }

    public void Visit<T>(DefTable table, IDataActionVisitor2<T> visitor, T arg)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, arg);
    }

    public void Visit<T1, T2>(DefTable table, IDataActionVisitor<T1, T2> visitor, T1 a1, T2 a2)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, a1, a2);
    }

    public void Visit<T1, T2>(DefTable table, IDataActionVisitor2<T1, T2> visitor, T1 a1, T2 a2)
    {
        var records = GenerationContext.Current.GetTableAllDataList(table);
        Visit(table, records, visitor, a1, a2);
    }

    public void Visit<T>(DefTable table, List<Record> records, IDataActionVisitor<T> visitor, T arg)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, arg);
        }
    }

    public void Visit<T>(DefTable table, List<Record> records, IDataActionVisitor2<T> visitor, T arg)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, table.ValueTType, arg);
        }
    }

    public void Visit<T1, T2>(DefTable table, List<Record> records, IDataActionVisitor<T1, T2> visitor, T1 a1, T2 a2)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, a1, a2);
        }
    }

    public void Visit<T1, T2>(DefTable table, List<Record> records, IDataActionVisitor2<T1, T2> visitor, T1 a1, T2 a2)
    {
        foreach (Record r in records)
        {
            DBean data = r.Data;
            data.Apply(visitor, table.ValueTType, a1, a2);
        }
    }
}
