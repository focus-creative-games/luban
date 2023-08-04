namespace Luban.Core.Schema;

public interface IRootSchemaLoader : ISchemaLoader
{
    public IReadOnlyList<SchemaFileInfo> ImportFiles { get; }
}