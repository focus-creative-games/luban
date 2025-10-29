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
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class LuaDeserializeMethodNameVisitor : ITypeFuncVisitor<string>
{
    public static LuaDeserializeMethodNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "readBool";
    }

    public string Accept(TByte type)
    {
        return "readByte";
    }

    public string Accept(TShort type)
    {
        return "readShort";
    }

    public string Accept(TInt type)
    {
        return "readInt";
    }

    public string Accept(TLong type)
    {
        return "readLong";
    }

    public string Accept(TFloat type)
    {
        return "readFloat";
    }

    public string Accept(TDouble type)
    {
        return "readDouble";
    }

    public string Accept(TEnum type)
    {
        return "readInt";
    }

    public string Accept(TString type)
    {
        return "readString";
    }

    public string Accept(TBean type)
    {
        return $"beans['{type.DefBean.FullName}']._deserialize";
    }

    public string Accept(TArray type)
    {
        return "readList";
    }

    public string Accept(TList type)
    {
        return "readList";
    }

    public string Accept(TSet type)
    {
        return "readSet";
    }

    public string Accept(TMap type)
    {
        return "readMap";
    }

    public string Accept(TDateTime type)
    {
        return "readLong";
    }
}
