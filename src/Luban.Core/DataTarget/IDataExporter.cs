
namespace Luban.Core.DataTarget;

public interface IDataExporter
{
    void Handle(GenerationContext ctx, IDataTarget dataTarget, OutputFileManifest manifest);
}