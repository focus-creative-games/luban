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
using Luban.DataLoader.Builtin.Lite;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader.Builtin.Excel.DataParser;


public class LiteParser : TextParserBase<LiteStream>
{
    protected override LiteStream CreateRawData(string dataStr)
    {
        return new LiteStream(dataStr);
    }

    protected override DBean ParseBean(TBean type, LiteStream rawData, TitleRow title)
    {
        return (DBean)type.Apply(LiteStreamDataCreator.Ins, rawData);
    }

    protected override DType ParseCollection(TType collectionType, LiteStream rawData, TitleRow title)
    {
        return collectionType.Apply(LiteStreamDataCreator.Ins, rawData);
    }

    protected override DMap ParseMap(TMap type, LiteStream rawData, TitleRow title)
    {
        return (DMap)type.Apply(LiteStreamDataCreator.Ins, rawData);
    }

    protected override KeyValuePair<DType, DType> ParseMapEntry(TMap type, LiteStream rawData, TitleRow title)
    {
        rawData.ReadStructOrCollectionBegin();
        var key = type.KeyType.Apply(LiteStreamDataCreator.Ins, rawData);
        var value = type.ValueType.Apply(LiteStreamDataCreator.Ins, rawData);
        rawData.ReadStructOrCollectionEnd();
        return new KeyValuePair<DType, DType>(key, value);
    }
}
