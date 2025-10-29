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
using Luban.Utils;

namespace Luban.Java.TypeVisitors;

public class JavaDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static JavaDeclaringTypeNameVisitor Ins { get; } = new();

    public virtual string Accept(TBool type)
    {
        return type.IsNullable ? "Boolean" : "boolean";
    }

    public virtual string Accept(TByte type)
    {
        return type.IsNullable ? "Byte" : "byte";
    }

    public virtual string Accept(TShort type)
    {
        return type.IsNullable ? "Short" : "short";
    }

    public virtual string Accept(TInt type)
    {
        return type.IsNullable ? "Integer" : "int";
    }

    public virtual string Accept(TLong type)
    {
        return type.IsNullable ? "Long" : "long";
    }

    public virtual string Accept(TFloat type)
    {
        return type.IsNullable ? "Float" : "float";
    }

    public virtual string Accept(TDouble type)
    {
        return type.IsNullable ? "Double" : "double";
    }

    public virtual string Accept(TEnum type)
    {
        string src = type.IsNullable ? "Integer" : "int";
        return type.DefEnum.TypeNameWithTypeMapper() ?? src;
    }

    public string Accept(TString type)
    {
        return "String";
    }

    public virtual string Accept(TDateTime type)
    {
        return type.IsNullable ? "Long" : "long";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.TypeNameWithTypeMapper() ?? type.DefBean.FullNameWithTopModule;
    }

    public string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public string Accept(TList type)
    {
        return $"java.util.ArrayList<{type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TSet type)
    {
        return $"java.util.HashSet<{type.ElementType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)}>";
    }

    public string Accept(TMap type)
    {
        return $"java.util.HashMap<{type.KeyType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)}, {type.ValueType.Apply(JavaDeclaringBoxTypeNameVisitor.Ins)}>";
    }
}
