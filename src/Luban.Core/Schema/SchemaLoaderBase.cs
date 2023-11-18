namespace Luban.Schema;

public abstract class SchemaLoaderBase : ISchemaLoader
{
    public string Type { get; set; }

    public ISchemaCollector Collector { get; set; }

    public abstract void Load(string fileName);
}
