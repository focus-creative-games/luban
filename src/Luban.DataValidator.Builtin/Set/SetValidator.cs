using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Set;

[Validator("set")]
public class SetValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    private LongDataSet _longSet;
    private StringDataSet _stringSet;

    private Func<DType, long> _longGetter;
    private Func<DType, string> _stringGetter;

    private readonly string _valueSetStr;

    public SetValidator()
    {
    }

    public override void Compile(DefField field)
    {
        switch (field.CType)
        {
            case TByte:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DByte) d).Value;
                break;
            }
            case TShort:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DShort) d).Value;
                break;
            }
            case TInt:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DInt) d).Value;
                break;
            }
            case TLong:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DLong) d).Value;
                break;
            }
            case TEnum:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DEnum) d).Value;
                break;
            }
            case TString:
            {
                _stringSet = new StringDataSet(Args);
                _stringGetter = d => ((DString) d).Value;
                break;
            }
            case TText:
            {
                _stringSet = new StringDataSet(Args);
                _stringGetter = d => ((DText) d).Value;
                break;
            }
            default:
            {
                throw new Exception($"set not support type:{field.CType}");
            }
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        if (type.IsNullable && data == null)
        {
            return;
        }
        if ((_longSet != null && !_longSet.Contains(_longGetter(data))) || (_stringSet != null && !_stringSet.Contains(_stringGetter(data))))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) 值不在set:{}中", RecordPath, data, Source, _valueSetStr);
        }
    }
}