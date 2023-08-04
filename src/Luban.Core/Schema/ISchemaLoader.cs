namespace Luban.Core.Schema;

public interface ISchemaLoader
{
    void Load(string fileName, ISchemaCollector collector);
}