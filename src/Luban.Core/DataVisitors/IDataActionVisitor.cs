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

public interface IDataActionVisitor<T>
{
    void Accept(DBool type, T x);

    void Accept(DByte type, T x);

    void Accept(DShort type, T x);

    void Accept(DInt type, T x);

    void Accept(DLong type, T x);

    void Accept(DFloat type, T x);

    void Accept(DDouble type, T x);

    void Accept(DEnum type, T x);

    void Accept(DString type, T x);

    void Accept(DDateTime type, T x);

    void Accept(DBean type, T x);

    void Accept(DArray type, T x);

    void Accept(DList type, T x);

    void Accept(DSet type, T x);

    void Accept(DMap type, T x);
}

public interface IDataActionVisitor<T1, T2>
{
    void Accept(DBool type, T1 x, T2 y);

    void Accept(DByte type, T1 x, T2 y);

    void Accept(DShort type, T1 x, T2 y);

    void Accept(DInt type, T1 x, T2 y);

    void Accept(DLong type, T1 x, T2 y);

    void Accept(DFloat type, T1 x, T2 y);

    void Accept(DDouble type, T1 x, T2 y);

    void Accept(DEnum type, T1 x, T2 y);

    void Accept(DString type, T1 x, T2 y);

    void Accept(DDateTime type, T1 x, T2 y);

    void Accept(DBean type, T1 x, T2 y);

    void Accept(DArray type, T1 x, T2 y);

    void Accept(DList type, T1 x, T2 y);

    void Accept(DSet type, T1 x, T2 y);

    void Accept(DMap type, T1 x, T2 y);
}
