using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Validators
{
    [Validator("size")]
    internal class SizeValidator : IValidator
    {
        private Range _range;

        public SizeValidator(TType type, string rule)
        {
            _range = new Range(rule);
        }

        public void Compile(DefFieldBase def)
        {
            _range.Compile();
        }

        private static string Source => ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;

        public void Validate(ValidatorContext ctx, TType type, DType data)
        {
            var assembly = ctx.Assembly;
            void LogError(int size)
            {
                assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) size:{3},但要求为 {4} ", ValidatorContext.CurrentRecordPath, data, Source, size, _range.RawStr);
            }

            if (type.IsNullable && data == null)
            {
                return;
            }

            switch (data)
            {
                case DArray b:
                {
                    if (!_range.CheckInLongRange(b.Datas.Count))
                    {
                        LogError(b.Datas.Count);
                        return;
                    }
                    break;
                }
                case DList b:
                {
                    if (!_range.CheckInLongRange(b.Datas.Count))
                    {
                        LogError(b.Datas.Count);
                        return;
                    }
                    break;
                }
                case DSet b:
                {
                    if (!_range.CheckInLongRange(b.Datas.Count))
                    {
                        LogError(b.Datas.Count);
                        return;
                    }
                    break;
                }
                case DMap b:
                {
                    if (!_range.CheckInLongRange(b.Datas.Count))
                    {
                        LogError(b.Datas.Count);
                        return;
                    }
                    break;
                }
                default:
                {
                    assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) 不支持 size", ValidatorContext.CurrentRecordPath, data, Source);
                    return;
                }
            }
        }
    }
}
