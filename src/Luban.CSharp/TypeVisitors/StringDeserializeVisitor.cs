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

namespace Luban.CSharp.TypeVisitors;

public class StringDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static StringDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string strName, string varName)
    {
        return $"{varName} = bool.Parse({strName});";
    }

    public string Accept(TByte type, string strName, string varName)
    {
        return $"{varName} = byte.Parse({strName});";
    }

    public string Accept(TShort type, string strName, string varName)
    {
        return $"{varName} = short.Parse({strName});";
    }

    public string Accept(TInt type, string strName, string varName)
    {
        return $"{varName} = int.Parse({strName});";
    }

    public string Accept(TLong type, string strName, string varName)
    {
        return $"{varName} = long.Parse({strName});";
    }

    public string Accept(TFloat type, string strName, string varName)
    {
        return $"{varName} = float.Parse({strName});";
    }

    public string Accept(TDouble type, string strName, string varName)
    {
        return $"{varName} = double.Parse({strName});";
    }

    public string Accept(TEnum type, string strName, string varName)
    {
        return $"{varName} = ({type.Apply(DeclaringTypeNameVisitor.Ins)})int.Parse({strName});";
    }

    public string Accept(TString type, string strName, string varName)
    {
        return $"{varName} = {strName};";
    }

    public string Accept(TDateTime type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TBean type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TArray type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TList type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TSet type, string strName, string varName)
    {
        throw new NotSupportedException();
    }

    public string Accept(TMap type, string strName, string varName)
    {
        throw new NotSupportedException();
    }
}
