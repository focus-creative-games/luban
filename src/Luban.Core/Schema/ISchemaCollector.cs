using Luban.RawDefs;

namespace Luban.Schema;

public interface ISchemaCollector
{
    void Load(string schemaPath);
    
    RawAssembly CreateRawAssembly();
    
    
    void Add(RawTable table);

    void Add(RawBean bean);

    void Add(RawEnum @enum);
    
    void Add(RawGroup group);
    
    void Add(RawRefGroup refGroup);
    
    void Add(RawTarget target);
}