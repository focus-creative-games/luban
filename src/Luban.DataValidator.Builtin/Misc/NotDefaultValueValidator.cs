using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Misc;

[Validator("not-default")]
public class NotDefaultValueValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public override void Compile(DefField field, TType type)
    {

    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        if (data.Apply(IsDefaultValueVisitor.Ins))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) 是一个默认值", DataValidatorContext.CurrentRecordPath, data, Source);
            GenerationContext.Current.LogValidatorFail(this);
        }
    }
}
