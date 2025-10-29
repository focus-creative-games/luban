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

using System.Text;
using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Neo.IronLua;

namespace Luban.DataLoader.Builtin.Lua;

[DataLoader("lua")]
public class LuaDataSource : DataLoaderBase
{
    public static Neo.IronLua.Lua LuaManager { get; } = new();

    private LuaGlobal _env;
    private LuaTable _dataTable;

    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        RawUrl = rawUrl;
        _env = LuaManager.CreateEnvironment();
        _dataTable = (LuaTable)_env.DoChunk(new StreamReader(stream, Encoding.UTF8), rawUrl)[0];

        if (!string.IsNullOrEmpty(sheetName))
        {
            if (sheetName.StartsWith("*"))
            {
                sheetName = sheetName.Substring(1);
            }
            if (!string.IsNullOrEmpty(sheetName))
            {
                foreach (var subField in sheetName.Split('.'))
                {
                    _dataTable = (LuaTable)_dataTable[subField];
                }
            }
        }
    }

    public override List<Record> ReadMulti(TBean type)
    {
        var records = new List<Record>();

        foreach (LuaTable t in _dataTable.Values.Values)
        {
            Record r = ReadRecord(t, type);
            if (r != null)
            {
                records.Add(r);
            }
        }

        return records;
    }

    public override Record ReadOne(TBean type)
    {
        return ReadRecord(_dataTable, type);
    }

    private Record ReadRecord(LuaTable table, TBean type)
    {
        string tagName = table.GetValue(FieldNames.TagKey)?.ToString();
        if (DataUtil.IsIgnoreTag(tagName))
        {
            return null;
        }
        var data = (DBean)type.Apply(LuaDataCreator.Ins, table, type.DefBean.Assembly);
        var tags = DataUtil.ParseTags(tagName);
        return new Record(data, RawUrl, tags);
    }
}
