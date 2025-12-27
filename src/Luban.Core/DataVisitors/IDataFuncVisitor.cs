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

namespace Luban.DataVisitors;

public interface IDataFuncVisitor<TR>
{
    TR Accept(DBool type);

    TR Accept(DByte type);

    TR Accept(DShort type);

    TR Accept(DInt type);

    TR Accept(DLong type);

    TR Accept(DFloat type);

    TR Accept(DDouble type);

    TR Accept(DEnum type);

    TR Accept(DString type);

    TR Accept(DDateTime type);

    TR Accept(DBean type);

    TR Accept(DArray type);

    TR Accept(DList type);

    TR Accept(DSet type);

    TR Accept(DMap type);
}

public interface IDataFuncVisitor<T, TR>
{
    TR Accept(DBool type, T x);

    TR Accept(DByte type, T x);

    TR Accept(DShort type, T x);

    TR Accept(DInt type, T x);

    TR Accept(DLong type, T x);

    TR Accept(DFloat type, T x);

    TR Accept(DDouble type, T x);

    TR Accept(DEnum type, T x);

    TR Accept(DString type, T x);

    TR Accept(DDateTime type, T x);

    TR Accept(DBean type, T x);

    TR Accept(DArray type, T x);

    TR Accept(DList type, T x);

    TR Accept(DSet type, T x);

    TR Accept(DMap type, T x);
}

public interface IDataFuncVisitor<T1, T2, TR>
{
    TR Accept(DBool type, T1 x, T2 y);

    TR Accept(DByte type, T1 x, T2 y);

    TR Accept(DShort type, T1 x, T2 y);

    TR Accept(DInt type, T1 x, T2 y);
    TR Accept(DLong type, T1 x, T2 y);

    TR Accept(DFloat type, T1 x, T2 y);

    TR Accept(DDouble type, T1 x, T2 y);

    TR Accept(DEnum type, T1 x, T2 y);

    TR Accept(DString type, T1 x, T2 y);

    TR Accept(DDateTime type, T1 x, T2 y);

    TR Accept(DBean type, T1 x, T2 y);

    TR Accept(DArray type, T1 x, T2 y);

    TR Accept(DList type, T1 x, T2 y);

    TR Accept(DSet type, T1 x, T2 y);

    TR Accept(DMap type, T1 x, T2 y);
}

