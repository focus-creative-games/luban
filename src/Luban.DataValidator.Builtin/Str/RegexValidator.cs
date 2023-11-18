using System.Text.RegularExpressions;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Str;

[Validator("regex")]
public class RegexValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private Regex _regex;

    private Func<DType, string> _stringGetter;

    public RegexValidator()
    {
    }

    public override void Compile(DefField field, TType type)
    {
        _regex = new Regex(Args, RegexOptions.Compiled);
        switch (type)
        {
            case TString:
            {
                _stringGetter = d => ((DString)d).Value;
                break;
            }
            default:
            {
                throw new Exception($"type:{field.CType} field:{field} not support regex validator");
            }
        }
    }


    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        Validate(ctx, type.IsNullable, data);
    }

    private void Validate(DataValidatorContext ctx, bool nullable, DType data)
    {
        if (!_regex.IsMatch(_stringGetter(data)))
        {
            s_logger.Error($"记录 {RecordPath}:{data} (来自文件:{Source}) 不符合正则表达式：'{_regex}'");
            GenerationContext.Current.LogValidatorFail(this);
        }
    }
}
