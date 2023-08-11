using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Defs;
using Luban.FlatBuffers.TemplateExtensions;
using Luban.FlatBuffers.TypeVisitors;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.FlatBuffers.CodeTarget;

[CodeTarget("flatbuffers")]
public class FlatBuffersSchemaTarget : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override string FileSuffixName => "fbs";
    
    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.NoneCodeStyle;

    protected override string DefaultOutputFileName => "schema.fbs";

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new FlatBuffersTemplateExtension());
        
        var maps = CollectKeyValueEntry(GenerationContext.Current.ExportBeans).KeyValueEntries.Values;
        ctx.PushGlobal(new ScriptObject()
        {
            {"__maps", maps},
        });
    }
    
    private MapKeyValueEntryCollection CollectKeyValueEntry(List<DefBean> beans)
    {
        var c = new MapKeyValueEntryCollection();

        foreach (DefBean bean in beans)
        {
            CollectMapKeyValueEntriesVisitor.Ins.Accept(bean, c);
        }

        return c;
    }
}