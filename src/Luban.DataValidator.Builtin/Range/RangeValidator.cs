using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Range;

[Validator("range")]
public class RangeValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private LongRange _longRange;
    private DoubleRange _doubleRange;

    private Func<DType, long> _longGetter;
    private Func<DType, double> _doubleGetter;

    public RangeValidator()
    {
    }

    public override void Compile(DefField field, TType type)
    {
        switch (type)
        {
            case TByte:
            {
                _longRange = new LongRange(Args);
                _longGetter = d => ((DByte)d).Value;
                break;
            }
            case TShort:
            {
                _longRange = new LongRange(Args);
                _longGetter = d => ((DShort)d).Value;
                break;
            }
            case TInt:
            {
                _longRange = new LongRange(Args);
                _longGetter = d => ((DInt)d).Value;
                break;
            }
            case TLong:
            {
                _longRange = new LongRange(Args);
                _longGetter = d => ((DLong)d).Value;
                break;
            }
            case TFloat:
            {
                _doubleRange = new DoubleRange(Args);
                _doubleGetter = d => ((DFloat)d).Value;
                break;
            }
            case TDouble:
            {
                _doubleRange = new DoubleRange(Args);
                _doubleGetter = d => ((DDouble)d).Value;
                break;
            }
            default:
                throw new Exception($"range not support type:{type} field:{field}");
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        if ((_longRange != null && !_longRange.CheckInRange(_longGetter(data))) || (_doubleRange != null && !_doubleRange.CheckInRange(_doubleGetter(data))))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) 不在范围:{}内", DataValidatorContext.CurrentRecordPath, data, Source, Args);
            GenerationContext.Current.LogValidatorFail(this);
        }
    }
}
