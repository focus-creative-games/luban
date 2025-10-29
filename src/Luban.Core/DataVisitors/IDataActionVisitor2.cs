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

namespace Luban.DataVisitors;

public interface IDataActionVisitor2<T>
{
    void Accept(DBool data, TType type, T x);

    void Accept(DByte data, TType type, T x);

    void Accept(DShort data, TType type, T x);

    void Accept(DInt data, TType type, T x);

    void Accept(DLong data, TType type, T x);

    void Accept(DFloat data, TType type, T x);

    void Accept(DDouble data, TType type, T x);

    void Accept(DEnum data, TType type, T x);

    void Accept(DString data, TType type, T x);

    void Accept(DDateTime data, TType type, T x);

    void Accept(DBean data, TType type, T x);

    void Accept(DArray data, TType type, T x);

    void Accept(DList data, TType type, T x);

    void Accept(DSet data, TType type, T x);

    void Accept(DMap data, TType type, T x);
}

public interface IDataActionVisitor2<T1, T2>
{
    void Accept(DBool data, TType type, T1 x, T2 y);

    void Accept(DByte data, TType type, T1 x, T2 y);

    void Accept(DShort data, TType type, T1 x, T2 y);

    void Accept(DInt data, TType type, T1 x, T2 y);

    void Accept(DLong data, TType type, T1 x, T2 y);

    void Accept(DFloat data, TType type, T1 x, T2 y);

    void Accept(DDouble data, TType type, T1 x, T2 y);

    void Accept(DEnum data, TType type, T1 x, T2 y);

    void Accept(DString data, TType type, T1 x, T2 y);

    void Accept(DDateTime data, TType type, T1 x, T2 y);

    void Accept(DBean data, TType type, T1 x, T2 y);

    void Accept(DArray data, TType type, T1 x, T2 y);

    void Accept(DList data, TType type, T1 x, T2 y);

    void Accept(DSet data, TType type, T1 x, T2 y);

    void Accept(DMap data, TType type, T1 x, T2 y);
}
