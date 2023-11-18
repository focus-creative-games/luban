using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.Validator;

public interface IDataValidator
{
    string Args { get; set; }

    void Compile(DefField owner, TType type);

    void Validate(DataValidatorContext ctx, TType type, DType data);
}
