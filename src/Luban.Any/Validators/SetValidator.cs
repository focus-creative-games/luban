namespace Luban.Any.Validators;

[Validator("set")]
class SetValidator : IValidator
{
    private readonly TType _type;

    private readonly HashSet<DType> _datas;

    private readonly string _valueSetStr;

    public SetValidator(TType ttype, string param)
    {
        _type = ttype;
        _datas = new HashSet<DType>();
        _valueSetStr = DefUtil.TrimBracePairs(param);
    }

    public void Compile(DefField def)
    {
        foreach (var p in _valueSetStr.Split(',', ';'))
        {
            _datas.Add(_type.Apply(StringDataCreator.Ins, p));
        }
    }

    public void Validate(ValidatorContext ctx, TType type, DType data)
    {
        if (type.IsNullable && data == null)
        {
            return;
        }
        if (!_datas.Contains(data))
        {
            ctx.Assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) 值不在set:{3}中", ValidatorContext.CurrentRecordPath, data,
                ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source, _valueSetStr);
        }
    }
}