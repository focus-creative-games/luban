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

﻿using Luban.Defs;
using Luban.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class DeclaringCollectionRefNameVisitor : ITypeFuncVisitor<string>
{
    public static DeclaringCollectionRefNameVisitor Ins { get; } = new();
    public string Accept(TBool type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TByte type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TShort type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TInt type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TLong type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TFloat type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDouble type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TEnum type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TString type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDateTime type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TArray type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins) + "[]";
        }
        throw new Exception($"解析'{type.ElementType}[]' 的ref失败");
    }

    public string Accept(TList type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstStrings.ListTypeName}<{refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.ListTypeName}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TSet type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstStrings.HashSetTypeName}<{refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.HashSetTypeName}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TMap type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}, {refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.HashMapTypeName}<{type.KeyType}, {type.ValueType}>' 的ref失败");
    }
    private static DefTable GetCollectionRefTable(TType type)
    {
        var refTag = type.GetTag("ref");
        if (refTag == null)
        {
            refTag = type.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return null;
        }
        if (GenerationContext.Current.Assembly.GetCfgTable(refTag.Replace("?", "")) is { } cfgTable)
        {
            return cfgTable;
        }
        return null;
    }
}
