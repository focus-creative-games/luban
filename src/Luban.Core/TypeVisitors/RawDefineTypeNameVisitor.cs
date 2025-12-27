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

public class RawDefineTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static RawDefineTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "byte";
    }

    public string Accept(TShort type)
    {
        return "short";
    }

    public string Accept(TInt type)
    {
        return "int";
    }

    public string Accept(TLong type)
    {
        return "long";
    }

    public string Accept(TFloat type)
    {
        return "float";
    }

    public string Accept(TDouble type)
    {
        return "double";
    }

    public string Accept(TEnum type)
    {
        return type.DefEnum.FullName;
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public virtual string Accept(TDateTime type)
    {
        return "datetime";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.FullName;
    }

    public string Accept(TArray type)
    {
        return $"array,{type.ElementType.Apply(this)}";
    }

    public string Accept(TList type)
    {
        return $"list,{type.ElementType.Apply(this)}";
    }

    public string Accept(TSet type)
    {
        return $"set,{type.ElementType.Apply(this)}";
    }

    public string Accept(TMap type)
    {
        return $"map,{type.KeyType.Apply(this)},{type.ValueType.Apply(this)}";
    }
}
