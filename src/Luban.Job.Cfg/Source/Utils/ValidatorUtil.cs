using Luban.Job.Cfg.Validators;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    static class ValidatorUtil
    {
        private static void CreateValidatorsForType(TType type)
        {
            foreach (var valName in ValidatorFactory.ValidatorNames)
            {
                if (type.Tags.TryGetValue(valName, out var valValue))
                {
                    type.Processors.Add(ValidatorFactory.Create(type, valName, valValue));
                }
            }
        }

        private static void CreateValidatorsForArrayLike(TType containerType, TType elementType)
        {
            CreateValidatorsForType(elementType);
            CreateValidatorsForType(containerType);
        }

        public static void CreateValidators(TType type)
        {
            switch (type)
            {
                case TArray ta:
                {
                    CreateValidatorsForArrayLike(type, ta.ElementType);
                    break;
                }
                case TList ta:
                {
                    CreateValidatorsForArrayLike(type, ta.ElementType);
                    break;
                }
                case TSet ta:
                {
                    CreateValidatorsForArrayLike(type, ta.ElementType);
                    break;
                }
                case TMap ta:
                {
                    CreateValidatorsForType(ta.KeyType);
                    CreateValidatorsForType(ta.ValueType);
                    CreateValidatorsForType(type);
                    break;
                }
                default:
                {
                    CreateValidatorsForType(type);
                    break;
                }
            }
        }
    }
}
