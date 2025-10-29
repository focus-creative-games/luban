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

public class IsSimpleLiteralDataVisitor : IDataFuncVisitor<bool>
{
    public static IsSimpleLiteralDataVisitor Ins { get; } = new();

    public bool Accept(DBool type)
    {
        return true;
    }

    public bool Accept(DByte type)
    {
        return true;
    }

    public bool Accept(DShort type)
    {
        return true;
    }

    public bool Accept(DInt type)
    {
        return true;
    }

    public bool Accept(DLong type)
    {
        return true;
    }

    public bool Accept(DFloat type)
    {
        return true;
    }

    public bool Accept(DDouble type)
    {
        return true;
    }

    public bool Accept(DEnum type)
    {
        return true;
    }

    public bool Accept(DString type)
    {
        return true;
    }

    public bool Accept(DDateTime type)
    {
        return true;
    }

    public bool Accept(DBean type)
    {
        return false;
    }

    public bool Accept(DArray type)
    {
        return false;
    }

    public bool Accept(DList type)
    {
        return false;
    }

    public bool Accept(DSet type)
    {
        return false;
    }

    public bool Accept(DMap type)
    {
        return false;
    }
}
