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
