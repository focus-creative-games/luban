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
using Luban.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataVisitors;

public interface IDataFuncVisitor2<TR>
{
    TR Accept(DBool data, TType type);

    TR Accept(DByte data, TType type);

    TR Accept(DShort data, TType type);

    TR Accept(DInt data, TType type);

    TR Accept(DLong data, TType type);

    TR Accept(DFloat data, TType type);

    TR Accept(DDouble data, TType type);

    TR Accept(DEnum data, TType type);

    TR Accept(DString data, TType type);

    TR Accept(DDateTime data, TType type);

    TR Accept(DBean data, TType type);

    TR Accept(DArray data, TType type);

    TR Accept(DList data, TType type);

    TR Accept(DSet data, TType type);

    TR Accept(DMap data, TType type);
}

public interface IDataFuncVisitor2<T, TR>
{
    TR Accept(DBool data, TType type, T x);

    TR Accept(DByte data, TType type, T x);

    TR Accept(DShort data, TType type, T x);

    TR Accept(DInt data, TType type, T x);

    TR Accept(DLong data, TType type, T x);

    TR Accept(DFloat data, TType type, T x);

    TR Accept(DDouble data, TType type, T x);

    TR Accept(DEnum data, TType type, T x);

    TR Accept(DString data, TType type, T x);

    TR Accept(DDateTime data, TType type, T x);

    TR Accept(DBean data, TType type, T x);

    TR Accept(DArray data, TType type, T x);

    TR Accept(DList data, TType type, T x);

    TR Accept(DSet data, TType type, T x);

    TR Accept(DMap data, TType type, T x);
}

public interface IDataFuncVisitor2<T1, T2, TR>
{
    TR Accept(DBool data, TType type, T1 x, T2 y);

    TR Accept(DByte data, TType type, T1 x, T2 y);

    TR Accept(DShort data, TType type, T1 x, T2 y);

    TR Accept(DInt data, TType type, T1 x, T2 y);
    TR Accept(DLong data, TType type, T1 x, T2 y);

    TR Accept(DFloat data, TType type, T1 x, T2 y);

    TR Accept(DDouble data, TType type, T1 x, T2 y);

    TR Accept(DEnum data, TType type, T1 x, T2 y);

    TR Accept(DString data, TType type, T1 x, T2 y);

    TR Accept(DDateTime data, TType type, T1 x, T2 y);

    TR Accept(DBean data, TType type, T1 x, T2 y);

    TR Accept(DArray data, TType type, T1 x, T2 y);

    TR Accept(DList data, TType type, T1 x, T2 y);

    TR Accept(DSet data, TType type, T1 x, T2 y);

    TR Accept(DMap data, TType type, T1 x, T2 y);
}

