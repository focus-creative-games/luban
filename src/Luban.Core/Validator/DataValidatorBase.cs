using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.Validator;

public abstract class DataValidatorBase : IDataValidator
{
    public string Args { get; set; }

    public abstract void Compile(DefField field, TType type);

    public abstract void Validate(DataValidatorContext ctx, TType type, DType data);

    protected static string Source => DataValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;

    protected static string RecordPath => DataValidatorContext.CurrentRecordPath;
}
