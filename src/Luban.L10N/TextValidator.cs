using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Validator;

namespace Luban.L10N;

[Validator("text")]
public class TextValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    private ITextProvider _provider;

    private ITextProvider Provider
    {
        get
        {
            if (_provider == null)
            {
                string textProviderName = EnvManager.Current.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextProviderName, false, "default");
                _provider = L10NManager.Ins.GetOrCreateContextUniqueTextProvider(textProviderName);
            }
            return _provider;
        }
    }
    
    public override void Compile(DefField field, TType type)
    {
        if (type is not TString)
        {
            throw new Exception($"field:{field} text validator supports string type only");
        }

    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        string key = ((DString)data).Value;
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        if (!Provider.IsValidKey(key))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) 不是一个有效的文本key", DataValidatorContext.CurrentRecordPath, data, Source);
            GenerationContext.Current.LogValidatorFail(this); 
        }
    }
}