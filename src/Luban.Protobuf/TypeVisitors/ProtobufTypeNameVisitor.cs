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
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Protobuf.TypeVisitors;

public class ProtobufTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static ProtobufTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "int32";
    }

    public string Accept(TShort type)
    {
        return "int32";
    }

    public string Accept(TInt type)
    {
        return "int32";
    }

    public string Accept(TLong type)
    {
        return "int64";
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
        return TypeUtil.MakePbFullName(type.DefEnum.Namespace, type.DefEnum.Name);
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public string Accept(TDateTime type)
    {
        return "int64";
    }

    public string Accept(TBean type)
    {
        return TypeUtil.MakePbFullName(type.DefBean.Namespace, type.DefBean.Name);
    }

    public string Accept(TArray type)
    {
        if (type.ElementType.IsCollection)
        {
            throw new Exception("not support multi-dimension array type");
        }
        return $"{type.ElementType.Apply(this)}";
    }

    public string Accept(TList type)
    {
        if (type.ElementType.IsCollection)
        {
            throw new Exception("not support multi-dimension list type");
        }
        return $"{type.ElementType.Apply(this)}";
    }

    public string Accept(TSet type)
    {
        if (type.ElementType.IsCollection)
        {
            throw new Exception("not support multi-dimension set type");
        }
        return $"{type.ElementType.Apply(this)}";
    }

    public string Accept(TMap type)
    {
        if (type.ElementType.IsCollection)
        {
            throw new Exception("not support multi-dimension map type");
        }
        string key = type.KeyType is TEnum ? "int32" : (type.KeyType.Apply(this));
        return $"map<{key}, {type.ValueType.Apply(this)}>";
    }
}
