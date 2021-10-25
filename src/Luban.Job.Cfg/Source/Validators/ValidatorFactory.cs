using Luban.Job.Cfg.RawDefs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Validators
{
    static class ValidatorFactory
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly static List<string> s_validatorNames = new List<string>() { RefValidator.NAME, PathValidator.NAME, RangeValidator.NAME };

        public static List<string> ValidatorNames => s_validatorNames;

        public static IValidator Create(string type, string rule)
        {
            s_logger.Debug("== create validator {type}:{rule}", type, rule);
            switch (type)
            {
                case RefValidator.NAME:
                {
                    return new RefValidator(rule.Split(',').ToList());
                }
                case PathValidator.NAME:
                {
                    return new PathValidator(rule);//.Split(',').ToList());
                }
                case RangeValidator.NAME:
                {
                    return new RangeValidator(rule);
                }
                default:
                    throw new NotSupportedException("unknown validator type:" + type);
            }
        }
    }
}
