using Luban.Datas;
using Luban.DataValidator.Builtin.Range;
using Luban.Defs;
using Luban.Types;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Collection;

[Validator("size")]
public class SizeValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    private LongRange _range;

    private Func<DType, long> _sizeGetter;

    public SizeValidator()
    {

    }

    public override void Compile(DefField field)
    {
        _range = new LongRange(Args);
        _sizeGetter = field.CType switch
        {
            TList => d => ((DList)d).Datas.Count,
            TSet => d => ((DSet)d).Datas.Count,
            TMap => d => ((DMap)d).Datas.Count,
            TArray => d => ((DArray)d).Datas.Count,
            _ => throw new Exception($"type:{field.CType} not support size validator"),
        };
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        if (type.IsNullable && data == null)
        {
            return;
        }

        long size = _sizeGetter(data);
        if (!_range.CheckInRange(size))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) size:{},但要求为 {} ", DataValidatorContext.CurrentRecordPath, data, Source, size, _range.RawStr);
        }
    }
}