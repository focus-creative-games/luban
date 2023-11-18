using Luban.Defs;

namespace Luban.Validator;

public interface ITableValidator
{
    void Validate(DefTable table, List<Record> records);
}
