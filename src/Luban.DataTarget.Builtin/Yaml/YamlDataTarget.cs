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

using Luban.DataTarget;
using Luban.Defs;
using Luban.Utils;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Luban.DataExporter.Builtin.Yaml;

[DataTarget("yaml")]
public class YamlDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "yml";

    public YamlNode WriteAsArray(List<Record> datas)
    {

        var seqNode = new YamlSequenceNode();
        foreach (var d in datas)
        {
            seqNode.Add(d.Data.Apply(YamlDataVisitor.Ins));
        }
        return seqNode;
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var node = WriteAsArray(records);
        var ys = new YamlStream(new YamlDocument(node));
        var ms = new MemoryStream();
        var tw = new StreamWriter(ms);
        ys.Save(tw, false);
        tw.Flush();
        return CreateOutputFile($"{table.OutputDataFile}.{OutputFileExt}", Encoding.UTF8.GetString(DataUtil.StreamToBytes(ms)));
    }
}
