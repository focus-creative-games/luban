using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.L10N;

[Validator("text")]
public class TextValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public override void Compile(DefField field, TType type)
    {
        if (type is not TString)
        {
            throw new Exception($"field:{field} text validator supports string type only");
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        ITextProvider provider = GenerationContext.Current.TextProvider;
        // dont' check when convertTextKeyToValue is true
        if (provider == null || provider.ConvertTextKeyToValue)
        {
            return;
        }
        string key = ((DString)data).Value;
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        if (!provider.IsValidKey(key))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) 不是一个有效的文本key", DataValidatorContext.CurrentRecordPath, data, Source);
            GenerationContext.Current.LogValidatorFail(this);
        }
    }
}
