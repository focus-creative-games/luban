using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System;

namespace Luban.Job.Cfg.Validators
{
    class RangeValidator : IValidator
    {
        public const string NAME = "range";

        public string Name => NAME;

        private readonly string _str;

        private long? _min;
        private long? _max;

        private double? _mind;
        private double? _maxd;

        private bool _includeMinBound;

        private bool _includeMaxBound;

        public RangeValidator(string strRange)
        {
            _str = strRange.Trim();
        }

        private bool TryParse(string s, ref long? x)
        {
            s = s.Trim();
            if (string.IsNullOrEmpty(s))
            {
                x = null;
                return true;
            }
            else if (long.TryParse(s, out var v))
            {
                x = v;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryParse(string s, ref double? x)
        {
            s = s.Trim();
            if (string.IsNullOrEmpty(s))
            {
                x = null;
                return true;
            }
            else if (double.TryParse(s, out var v))
            {
                x = v;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Compile(DefField def)
        {
            void ThrowError()
            {
                throw new Exception($"结构:{ def.HostType.FullName } 字段: { def.Name}  range 定义:{_str} 不合法");
            }

            if (_str.Length <= 2)
            {
                ThrowError();
            }
            switch (_str[0])
            {
                case '[': _includeMinBound = true; break;
                case '(': _includeMinBound = false; break;
                default: ThrowError(); break;
            }
            switch (_str[^1])
            {
                case ']': _includeMaxBound = true; break;
                case ')': _includeMaxBound = false; break;
                default: ThrowError(); break;
            }

            var pars = _str[1..^1].Split(',');
            if (pars.Length != 2)
            {
                ThrowError();
            }

            bool p1 = TryParse(pars[0], ref _min);
            bool p2 = TryParse(pars[0], ref _mind);
            bool p3 = TryParse(pars[1], ref _max);
            bool p4 = TryParse(pars[1], ref _maxd);

            if ((!p1 && !p2) || (!p3 && !p4))
            {
                ThrowError();
            }
        }


        private bool CheckInLongRange(long x)
        {
            if (_min is long m && (_includeMinBound ? m > x : m >= x))
            {
                return false;
            }
            if (_max is long n && (_includeMaxBound ? n < x : n <= x))
            {
                return false;
            }
            return true;
        }

        private bool CheckInDoubleRange(double x)
        {
            if (_mind is double m && (_includeMinBound ? m > x : m >= x))
            {
                return false;
            }
            if (_maxd is double n && (_includeMaxBound ? n < x : n <= x))
            {
                return false;
            }
            return true;
        }

        public string Source => ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;

        public void Validate(ValidatorContext ctx, DType data, bool nullable)
        {
            var assembly = ctx.Assembly;
            void LogError()
            {
                assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) 不在范围:{3}内", ValidatorContext.CurrentRecordPath, data, Source, _str);
            }

            if (nullable && data == null)
            {
                return;
            }

            switch (data)
            {
                case DByte b:
                {
                    if (!CheckInLongRange(b.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFshort s:
                {
                    if (!CheckInLongRange(s.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DShort s:
                {
                    if (!CheckInLongRange(s.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DInt i:
                {
                    if (!CheckInLongRange(i.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFint i:
                {
                    if (!CheckInLongRange(i.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DLong l:
                {
                    if (!CheckInLongRange(l.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFlong fl:
                {
                    if (!CheckInLongRange(fl.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DFloat ff:
                {
                    if (!CheckInDoubleRange(ff.Value))
                    {
                        LogError();
                        return;
                    }
                    break;
                }
                case DDouble dd:
                {
                    if (!CheckInDoubleRange(dd.Value))
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
