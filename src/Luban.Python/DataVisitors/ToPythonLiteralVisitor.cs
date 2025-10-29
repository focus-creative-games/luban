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

using System.Text;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.Python.DataVisitors;

public class ToPythonLiteralVisitor : ToLiteralVisitorBase
{
    public static ToPythonLiteralVisitor Ins { get; } = new();

    public override string Accept(DBool type)
    {
        return type.Value ? "True" : "False";
    }

    public override string Accept(DBean type)
    {
        var x = new StringBuilder();
        var bean = type.ImplType;
        if (bean.IsAbstractType)
        {
            x.Append($"{{ \"_name\":\"{type.ImplType.Name}\",");
        }
        else
        {
            x.Append('{');
        }

        int index = 0;
        foreach (var f in type.Fields)
        {
            var defField = (DefField)type.ImplType.HierarchyFields[index++];
            if (f == null || !defField.NeedExport())
            {
                continue;
            }
            x.Append('\"').Append(defField.Name).Append('\"').Append(':');
            x.Append(f.Apply(this));
            x.Append(',');
        }
        x.Append('}');
        return x.ToString();
    }


    protected virtual void Append(List<DType> datas, StringBuilder x)
    {
        x.Append('[');
        int index = 0;
        foreach (var e in datas)
        {
            if (index > 0)
            {
                x.Append(',');
            }
            ++index;
            x.Append(e.Apply(this));
        }
        x.Append(']');
    }

    public override string Accept(DArray type)
    {
        var x = new StringBuilder();
        Append(type.Datas, x);
        return x.ToString();
    }

    public override string Accept(DList type)
    {
        var x = new StringBuilder();
        Append(type.Datas, x);
        return x.ToString();
    }

    public override string Accept(DSet type)
    {
        var x = new StringBuilder();
        Append(type.Datas, x);
        return x.ToString();
    }

    public override string Accept(DMap type)
    {
        var x = new StringBuilder();
        x.Append('{');
        int index = 0;
        foreach (var e in type.DataMap)
        {
            if (index > 0)
            {
                x.Append(',');
            }
            ++index;
            x.Append(e.Key.Apply(this));
            x.Append(':');
            x.Append(e.Value.Apply(this));
        }
        x.Append('}');
        return x.ToString();
    }
}
