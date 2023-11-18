using Luban.RawDefs;

namespace Luban.Schema;

public interface ISchemaCollector
{
    void Load(LubanConfig config);

    RawAssembly CreateRawAssembly();

    void Add(RawTable table);

    void Add(RawBean bean);

    void Add(RawEnum @enum);

    void Add(RawRefGroup refGroup);

}
