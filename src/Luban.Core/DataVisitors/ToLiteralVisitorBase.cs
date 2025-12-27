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
using Luban.Utils;

namespace Luban.DataVisitors;

public abstract class ToLiteralVisitorBase : IDataFuncVisitor<string>
{
    public virtual string Accept(DBool type)
    {
        return type.Value ? "true" : "false";
    }

    public string Accept(DByte type)
    {
        return type.Value.ToString();
    }

    public string Accept(DShort type)
    {
        return type.Value.ToString();
    }

    public string Accept(DInt type)
    {
        return type.Value.ToString();
    }

    public string Accept(DLong type)
    {
        return type.Value.ToString();
    }

    public string Accept(DFloat type)
    {
        return type.Value.ToString();
    }

    public string Accept(DDouble type)
    {
        return type.Value.ToString();
    }

    public virtual string Accept(DEnum type)
    {
        return type.Value.ToString();
    }

    public virtual string Accept(DString type)
    {
        return "\"" + DataUtil.EscapeString(type.Value) + "\"";
    }

    public virtual string Accept(DDateTime type)
    {
        return type.UnixTimeOfCurrentContext().ToString();
    }

    public abstract string Accept(DBean type);

    public abstract string Accept(DArray type);

    public abstract string Accept(DList type);

    public abstract string Accept(DSet type);

    public abstract string Accept(DMap type);
}
