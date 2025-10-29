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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public static class DataParserFactory
{
    public const string FORMAT_TAG_NAME = "format";

    private readonly static IDataParser s_streamParser = new StreamParser();
    private readonly static IDataParser s_liteParser = new LiteParser();
    private readonly static IDataParser s_jsonParser = new JsonParser();
    private readonly static IDataParser s_luaParser = new LuaParser();

    public static IDataParser GetDefaultDataParser()
    {
        return s_streamParser;
    }

    public static IDataParser GetDataParser(string parserType)
    {
        if (string.IsNullOrEmpty(parserType))
        {
            return null;
        }
        return parserType switch
        {
            "" => s_streamParser,
            "stream" => s_streamParser,
            "lite" => s_liteParser,
            "json" => s_jsonParser,
            "lua" => s_luaParser,
            _ => throw new NotSupportedException($"Unsupported data parser type: {parserType}")
        };
    }
}
