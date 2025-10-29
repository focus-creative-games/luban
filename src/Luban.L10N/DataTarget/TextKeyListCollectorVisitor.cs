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

﻿

using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;

namespace Luban.L10N.DataTarget;


/// <summary>
/// 检查 相同key的text,原始值必须相同
/// </summary>
public class TextKeyListCollectorVisitor : IDataActionVisitor2<TextKeyCollection>
{
    public static TextKeyListCollectorVisitor Ins { get; } = new TextKeyListCollectorVisitor();

    public void Accept(DBool data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DByte data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DShort data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DInt data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DLong data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DFloat data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DDouble data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DEnum data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DString data, TType type, TextKeyCollection x)
    {
        if (data != null && type.HasTag("text"))
        {
            x.AddKey(data.Value);
        }
    }

    public void Accept(DDateTime data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DBean data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DArray data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DList data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DSet data, TType type, TextKeyCollection x)
    {

    }

    public void Accept(DMap data, TType type, TextKeyCollection x)
    {

    }
}
