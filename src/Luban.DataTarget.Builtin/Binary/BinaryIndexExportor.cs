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

using Luban.Datas;
using Luban.Defs;
using Luban.Serialization;

namespace Luban.DataExporter.Builtin.Binary;

class BinaryIndexExportor
{
    public static BinaryIndexExportor Ins { get; } = new();

    public void WriteList(DefTable table, List<Record> datas, ByteBuf x)
    {
        x.WriteSize(datas.Count);
        var tableDataBuf = new ByteBuf(10 * 1024);
        tableDataBuf.WriteSize(datas.Count);

        foreach (var d in datas)
        {
            int offset = tableDataBuf.Size;
            d.Data.Apply(BinaryDataVisitor.Ins, tableDataBuf);

            string keyStr = "";
            foreach (IndexInfo index in table.IndexList)
            {
                DType key = d.Data.Fields[index.IndexFieldIdIndex];
                key.Apply(BinaryDataVisitor.Ins, x);
                keyStr += key.ToString() + ",";
            }
            x.WriteSize(offset);
            Console.WriteLine($"table:{table.Name} key:{keyStr} offset:{offset}");
        }

    }
}
