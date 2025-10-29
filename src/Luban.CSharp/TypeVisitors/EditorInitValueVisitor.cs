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

﻿using Luban.Types;

namespace Luban.CSharp.TypeVisitors;

public class EditorInitValueVisitor : CtorDefaultValueVisitor
{
    public new static EditorInitValueVisitor Ins { get; } = new();

    public override string Accept(TEnum type)
    {
        return $"{(type.DefEnum.Items.Count > 0 ? $"{type.Apply(EditorDeclaringTypeNameVisitor.Ins)}." + type.DefEnum.Items[0].Name : "default")}";
    }

    public override string Accept(TDateTime type)
    {
        return "\"1970-01-01 00:00:00\"";
    }

    public override string Accept(TBean type)
    {
        return type.IsNullable || type.DefBean.IsAbstractType ? "default" : $"new {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}()";
    }

    public override string Accept(TArray type)
    {
        return $"System.Array.Empty<{type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TList type)
    {
        return $"new {ConstStrings.ListTypeName}<{type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TSet type)
    {
        return $"new {ConstStrings.HashSetTypeName}<{type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }

    public override string Accept(TMap type)
    {
        return $"new {ConstStrings.HashMapTypeName}<{type.KeyType.Apply(EditorDeclaringTypeNameVisitor.Ins)},{type.ValueType.Apply(EditorDeclaringTypeNameVisitor.Ins)}>()";
    }
}
