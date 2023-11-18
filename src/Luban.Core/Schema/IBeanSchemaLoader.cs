using Luban.RawDefs;

namespace Luban.Schema;

public interface IBeanSchemaLoader
{
    RawBean Load(string fileName, string beanFullName);
}
