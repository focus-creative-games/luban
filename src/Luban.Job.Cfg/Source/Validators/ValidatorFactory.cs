using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Validators
{
    static class ValidatorFactory
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly static List<string> s_validatorNames = new List<string>()
        {
            RefValidator.NAME,
            PathValidator.NAME,
            RangeValidator.NAME,
            SizeValidator.NAME
        };

        public static List<string> ValidatorNames => s_validatorNames;

        public static IValidator Create(TType field, string type, string rule)
        {
            s_logger.Debug("== create validator {type}:{rule}", type, rule);
            switch (type)
            {
                case RefValidator.NAME:
                {
                    return new RefValidator(field, rule.Split(',').ToList());
                }
                case PathValidator.NAME:
                {
                    return new PathValidator(field, rule);//.Split(',').ToList());
                }
                case RangeValidator.NAME:
                {
                    return new RangeValidator(field, rule);
                }
                case SizeValidator.NAME:
                {
                    return new SizeValidator(field, rule);
                }
                default:
                throw new NotSupportedException("unknown validator type:" + type);
            }
        }
    }
}
