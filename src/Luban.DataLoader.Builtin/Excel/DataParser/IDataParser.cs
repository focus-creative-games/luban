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
using Luban.Types;
using Luban.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public interface IDataParser
{
    DType ParseAny(TType type, List<Cell> cells, TitleRow title);

    DBean ParseBean(TBean type, List<Cell> cells, TitleRow title);

    List<DType> ParseCollectionElements(TType collectionType, List<Cell> cells, TitleRow title);

    DMap ParseMap(TMap type, List<Cell> cells, TitleRow title);

    KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title);
}

public abstract class DataParserBase : IDataParser
{
    public abstract DType ParseAny(TType type, List<Cell> cells, TitleRow title);
    public abstract DBean ParseBean(TBean type, List<Cell> cells, TitleRow title);
    public abstract List<DType> ParseCollectionElements(TType collectionType, List<Cell> cells, TitleRow title);
    public abstract DMap ParseMap(TMap type, List<Cell> cells, TitleRow title);
    public abstract KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title);
}
