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
            Encoding = System.Text.Encoding.UTF8,
        };
        var ms = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(ms, xwSetting);
        WriteAsArray(records, xmlWriter);
        xmlWriter.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ms),
        };
    }
}
