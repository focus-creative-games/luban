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

﻿using Google.Protobuf;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Protobuf.DataVisitors;

public class ProtobufWireTypeVisitor : ITypeFuncVisitor<WireFormat.WireType>
{
    public static ProtobufWireTypeVisitor Ins { get; } = new();

    public WireFormat.WireType Accept(TBool type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TByte type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TShort type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TInt type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TLong type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TFloat type)
    {
        return WireFormat.WireType.Fixed32;
    }

    public WireFormat.WireType Accept(TDouble type)
    {
        return WireFormat.WireType.Fixed64;
    }

    public WireFormat.WireType Accept(TEnum type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TString type)
    {
        return WireFormat.WireType.LengthDelimited;
    }

    public WireFormat.WireType Accept(TDateTime type)
    {
        return WireFormat.WireType.Varint;
    }

    public WireFormat.WireType Accept(TBean type)
    {
        return WireFormat.WireType.LengthDelimited;
    }

    public WireFormat.WireType Accept(TArray type)
    {
        //return WireFormat.WireType.LengthDelimited;
        throw new Exception("not support multi-dimension array wire type");
    }

    public WireFormat.WireType Accept(TList type)
    {
        throw new Exception("not support multi-dimension list wire type");
    }

    public WireFormat.WireType Accept(TSet type)
    {
        throw new Exception("not support multi-dimension set wire type");
    }

    public WireFormat.WireType Accept(TMap type)
    {
        //return WireFormat.WireType.LengthDelimited;
        throw new Exception("not support multi-dimension map wire type");
    }
}
