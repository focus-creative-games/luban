namespace Luban.Any.Validators;

static class ValidatorFactory
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    static ValidatorFactory()
    {
        foreach (var type in Bright.Common.ReflectionUtil.GetCallingTypesByAttr(typeof(ValidatorAttribute)))
        {
            var va = (ValidatorAttribute)type.GetCustomAttributes(typeof(ValidatorAttribute), false)[0];
            s_validators.Add(va.Name, type);
            s_validatorNames.Add(va.Name);
        }
    }

    private static readonly Dictionary<string, Type> s_validators = new();

    private static readonly List<string> s_validatorNames = new();

    public static List<string> ValidatorNames => s_validatorNames;

    public static IValidator Create(TType ttype, string type, string rule)
    {
        s_logger.Debug("== create validator {type}:{rule}", type, rule);
        if (s_validators.TryGetValue(type, out var validatorType))
        {
            return (IValidator)System.Activator.CreateInstance(validatorType, ttype, rule);
        }
        else
        {
            throw new NotSupportedException("unknown validator type:" + type);
        }
    }
}