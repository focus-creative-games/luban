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

﻿using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Lite;

[DataLoader("lit")]
public class LiteDataSource : DataLoaderBase
{
    private LiteStream _liteStream;

    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        var reader = new StreamReader(stream, Encoding.UTF8);
        _liteStream = new LiteStream(reader.ReadToEnd());
    }

    public override List<Record> ReadMulti(TBean type)
    {
        var records = new List<Record>();
        _liteStream.ReadStructOrCollectionBegin();
        while (_liteStream.IsEndOfStructOrCollection())
        {
            var record = ReadRecord(_liteStream, type);
            records.Add(record);
        }
        _liteStream.ReadStructOrCollectionEnd();
        return records;
    }

    public override Record ReadOne(TBean type)
    {
        return ReadRecord(_liteStream, type);
    }

    private Record ReadRecord(LiteStream stream, TBean type)
    {
        var data = (DBean)type.Apply(LiteStreamDataCreator.Ins, stream);
        return new Record(data, RawUrl, null);
    }
}
