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

using System.Text.Json;
using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.Json;

[DataLoader("json")]
public class JsonDataSource : DataLoaderBase
{
    private JsonElement _data;

    public override void Load(string rawUrl, string sheetOrFieldName, Stream stream)
    {
        RawUrl = rawUrl;
        this._data = JsonDocument.Parse(stream, new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        }).RootElement;

        if (!string.IsNullOrEmpty(sheetOrFieldName))
        {
            if (sheetOrFieldName.StartsWith("*"))
            {
                sheetOrFieldName = sheetOrFieldName.Substring(1);
            }
            if (!string.IsNullOrEmpty(sheetOrFieldName))
            {
                foreach (var subField in sheetOrFieldName.Split('.'))
                {
                    _data = _data.GetProperty(subField);
                }
            }
        }
    }

    public override List<Record> ReadMulti(TBean type)
    {
        var records = new List<Record>();
        foreach (var ele in _data.EnumerateArray())
        {
            Record rec = ReadRecord(ele, type);
            if (rec != null)
            {
                records.Add(rec);
            }
        }
        return records;
    }

    private Record ReadRecord(JsonElement ele, TBean type)
    {
        List<string> tags;
        if (ele.TryGetProperty(FieldNames.TagKey, out var tagEle))
        {
            var tagName = tagEle.GetString();
            if (DataUtil.IsIgnoreTag(tagName))
            {
                return null;
            }
            tags = DataUtil.ParseTags(tagName);
        }
        else
        {
            tags = null;
        }

        var data = (DBean)type.Apply(JsonDataCreator.Ins, ele, type.DefBean.Assembly);
        return new Record(data, RawUrl, tags);
    }

    public override Record ReadOne(TBean type)
    {
        return ReadRecord(_data, type);
    }
}
