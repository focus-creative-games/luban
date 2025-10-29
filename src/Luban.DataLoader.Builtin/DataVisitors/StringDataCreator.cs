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
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

class StringDataCreator : ITypeFuncVisitor<string, DType>
{
    public static StringDataCreator Ins { get; } = new();

    public DType Accept(TBool type, string x)
    {
        if (bool.TryParse(x, out var b))
        {
            return DBool.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是bool类型");
        }
    }

    public DType Accept(TByte type, string x)
    {
        if (byte.TryParse(x, out var b))
        {
            return DByte.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是byte类型");
        }
    }

    public DType Accept(TShort type, string x)
    {
        if (short.TryParse(x, out var b))
        {
            return DShort.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是short类型");
        }
    }

    public DType Accept(TInt type, string x)
    {
        if (int.TryParse(x, out var b))
        {
            return DInt.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是int类型");
        }
    }

    public DType Accept(TLong type, string x)
    {
        if (long.TryParse(x, out var b))
        {
            return DLong.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是long类型");
        }
    }

    public DType Accept(TFloat type, string x)
    {
        if (float.TryParse(x, out var b))
        {
            return DFloat.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是float类型");
        }
    }

    public DType Accept(TDouble type, string x)
    {
        if (double.TryParse(x, out var b))
        {
            return DDouble.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是double类型");
        }
    }

    public DType Accept(TEnum type, string x)
    {
        return new DEnum(type, x);
    }

    public DType Accept(TString type, string x)
    {
        return DString.ValueOf(type, x);
    }

    public DType Accept(TBean type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TArray type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TList type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TSet type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TMap type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TDateTime type, string x)
    {
        throw new NotSupportedException();
    }
}
