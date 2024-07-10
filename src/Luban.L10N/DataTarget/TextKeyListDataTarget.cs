using Luban.DataTarget;
using Luban.DataVisitors;
using Luban.Defs;
using System.Text;

namespace Luban.L10N.DataTarget;

[DataTarget("text-list")]
internal class TextKeyListDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "txt";

    public override bool ExportAllRecords => true;

    public override AggregationType AggregationType => AggregationType.Tables;

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        throw new NotImplementedException();
    }

    public override OutputFile ExportTables(List<DefTable> tables)
    {
        var textCollection = new TextKeyCollection();

        var visitor = new DataActionHelpVisitor2<TextKeyCollection>(TextKeyListCollectorVisitor.Ins);

        foreach (var table in tables)
        {
            TableVisitor.Ins.Visit(table, visitor, textCollection);
        }

        var keys = textCollection.Keys.ToList();
        keys.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));
        var content = string.Join("\n", keys);

        string outputFile = EnvManager.Current.GetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NTextListFile, false);

        return new OutputFile { File = outputFile, Content = content, Encoding = FileEncoding };
    }
}
