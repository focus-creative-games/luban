using Luban.DataTarget;

namespace Luban.DataExporter.Builtin;

[DataExporter("null")]
public class NullDataExporter : DataExporterBase
{
    public override void Handle(GenerationContext ctx, IDataTarget dataTarget, OutputFileManifest manifest)
    {

    }
}
