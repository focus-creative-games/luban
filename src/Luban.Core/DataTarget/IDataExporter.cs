
namespace Luban.DataTarget;

public interface IDataExporter
{
    void Handle(GenerationContext ctx, IDataTarget dataTarget, OutputFileManifest manifest);
}
