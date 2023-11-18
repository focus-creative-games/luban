namespace Luban.Schema;

public interface ISchemaLoader
{
    string Type { get; set; }

    ISchemaCollector Collector { get; set; }

    void Load(string fileName);
}
