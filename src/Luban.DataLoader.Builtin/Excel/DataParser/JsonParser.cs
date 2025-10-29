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

﻿using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public class JsonParser : TextParserBase<JsonElement>
{
    protected override JsonElement CreateRawData(string dataStr)
    {
        return JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(dataStr)), new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        }).RootElement;
    }

    protected override DBean ParseBean(TBean type, JsonElement rawData, TitleRow title)
    {
        return (DBean)type.Apply(JsonDataCreator.Ins, rawData, type.DefBean.Assembly);
    }

    protected override DType ParseCollection(TType collectionType, JsonElement rawData, TitleRow title)
    {
        return collectionType.Apply(JsonDataCreator.Ins, rawData, GenerationContext.Current.Assembly);
    }

    protected override DMap ParseMap(TMap type, JsonElement rawData, TitleRow title)
    {
        return (DMap)type.Apply(JsonDataCreator.Ins, rawData, GenerationContext.Current.Assembly);
    }

    protected override KeyValuePair<DType, DType> ParseMapEntry(TMap type, JsonElement rawData, TitleRow title)
    {
        if (rawData.ValueKind != JsonValueKind.Array)
        {
            throw new Exception($"json map entry must be array");
        }
        if (rawData.GetArrayLength() != 2)
        {
            throw new Exception($"json map entry must be [key,value] array");
        }
        DType key = type.KeyType.Apply(JsonDataCreator.Ins, rawData[0], GenerationContext.Current.Assembly);
        DType value = type.ValueType.Apply(JsonDataCreator.Ins, rawData[1], GenerationContext.Current.Assembly);
        return new KeyValuePair<DType, DType>(key, value);
    }
}
