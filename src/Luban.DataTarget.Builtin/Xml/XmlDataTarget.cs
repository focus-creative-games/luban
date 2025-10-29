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
using System.Xml;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Xml;

[DataTarget("xml")]
public class XmlDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "xml";

    public void WriteAsArray(List<Record> datas, XmlWriter w)
    {
        w.WriteStartDocument();
        w.WriteStartElement("table");
        foreach (var d in datas)
        {
            w.WriteStartElement("record");
            d.Data.Apply(XmlDataVisitor.Ins, w);
            w.WriteEndElement();
        }
        w.WriteEndElement();
        w.WriteEndDocument();
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var xwSetting = new XmlWriterSettings()
        {
            Indent = true,
            Encoding = Encoding.UTF8,
        };
        var ms = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(ms, xwSetting);
        WriteAsArray(records, xmlWriter);
        xmlWriter.Flush();
        return CreateOutputFile($"{table.OutputDataFile}.{OutputFileExt}", Encoding.UTF8.GetString(DataUtil.StreamToBytes(ms)));
    }
}
