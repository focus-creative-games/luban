using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using System;

namespace Luban.Job.Cfg.Validators
{
    [Validator("range")]
    class RangeValidator : IValidator
    {
        public TType Type { get; }

        private Range _range;

        public RangeValidator(TType type, string strRange)
        {
            Type = type;
            _range = new Range(strRange);
        }

        public void Compile(DefFieldBase def)
        {
            _range.Compile();
        }

        public string Source => ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;

        public void Validate(ValidatorContext ctx, TType type, DType data)
        {
            var assembly = ctx.Assembly;
            void LogError()
            {
                assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) 不在范围:{3}内", ValidatorContext.CurrentRecordPath, data, Source, _range.RawStr);
            }

            if (type.IsNullable && data == null)
            {
                return;
            }

            switch (data)
            {
                case DByte b:
                {
                    if (!_range.CheckInLongRange(b.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFshort s:
                {
                    if (!_range.CheckInLongRange(s.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DShort s:
                {
                    if (!_range.CheckInLongRange(s.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DInt i:
                {
                    if (!_range.CheckInLongRange(i.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFint i:
                {
                    if (!_range.CheckInLongRange(i.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DLong l:
                {
                    if (!_range.CheckInLongRange(l.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFlong fl:
                {
                    if (!_range.CheckInLongRange(fl.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFloat ff:
                {
                    if (!_range.CheckInDoubleRange(ff.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DDouble dd:
                {
                    if (!_range.CheckInDoubleRange(dd.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                default:
                {
                    assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) 不支持 range", ValidatorContext.CurrentRecordPath, data, Source);
                    return;
                }
            }
        }
    }
}
