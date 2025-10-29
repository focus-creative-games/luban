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

﻿using Luban.Datas;
using Luban.DataTransformer;
using Luban.DataVisitors;
using Luban.Types;
using Luban.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.L10N;

public class TextKeyToValueTransformer : DataTransfomerBase, IDataFuncVisitor2<DType>
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly ITextProvider _provider;

    public TextKeyToValueTransformer(ITextProvider provider)
    {
        _provider = provider;
    }

    DType IDataFuncVisitor2<DType>.Accept(DString data, TType type)
    {
        if (string.IsNullOrEmpty(data.Value) || !type.HasTag("text"))
        {
            return data;
        }
        if (_provider.TryGetText(data.Value, out var text))
        {
            return DString.ValueOf(type, text);
        }
        s_logger.Error("can't find target language text of text id:{} ", data.Value);
        //_provider.AddUnknownKey(data.Value);
        return data;
    }
}
