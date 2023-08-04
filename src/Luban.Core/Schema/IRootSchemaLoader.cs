namespace Luban.Schema;

public interface IRootSchemaLoader : ISchemaLoader
{
    public IReadOnlyList<SchemaFileInfo> ImportFiles { get; }
}