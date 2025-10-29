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

using Luban.Types;

namespace Luban.TypeVisitors;

public interface ITypeActionVisitor<T>
{
    void Accept(TBool type, T x);

    void Accept(TByte type, T x);

    void Accept(TShort type, T x);

    void Accept(TInt type, T x);

    void Accept(TLong type, T x);

    void Accept(TFloat type, T x);

    void Accept(TDouble type, T x);

    void Accept(TEnum type, T x);

    void Accept(TString type, T x);

    void Accept(TDateTime type, T x);

    void Accept(TBean type, T x);

    void Accept(TArray type, T x);

    void Accept(TList type, T x);

    void Accept(TSet type, T x);

    void Accept(TMap type, T x);
}

public interface ITypeActionVisitor<T1, T2>
{
    void Accept(TBool type, T1 x, T2 y);

    void Accept(TByte type, T1 x, T2 y);

    void Accept(TShort type, T1 x, T2 y);

    void Accept(TInt type, T1 x, T2 y);

    void Accept(TLong type, T1 x, T2 y);

    void Accept(TFloat type, T1 x, T2 y);

    void Accept(TDouble type, T1 x, T2 y);

    void Accept(TEnum type, T1 x, T2 y);

    void Accept(TString type, T1 x, T2 y);

    void Accept(TDateTime type, T1 x, T2 y);

    void Accept(TBean type, T1 x, T2 y);

    void Accept(TArray type, T1 x, T2 y);

    void Accept(TList type, T1 x, T2 y);

    void Accept(TSet type, T1 x, T2 y);

    void Accept(TMap type, T1 x, T2 y);
}
