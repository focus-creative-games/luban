using Luban.Job.Cfg.RawDefs;
using System;
using System.Linq;

namespace Luban.Job.Cfg.Validators
{
    static class ValidatorFactory
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
        public static IValidator Create(Validator validator)
        {
            s_logger.Debug("== create validator {type}:{rule}", validator.Type, validator.Rule);
            switch (validator.Type)
            {
                case RefValidator.NAME:
                {
                    return new RefValidator(validator.Rule.Split(',').ToList());
                }
                case PathValidator.NAME:
                {
                    return new PathValidator(validator.Rule);//.Split(',').ToList());
                }
                case RangeValidator.NAME:
                {
                    return new RangeValidator(validator.Rule);
                }
                default:
                    throw new NotSupportedException("unknown validator type:" + validator.Type);
            }
        }
    }
}
