using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Validators
{
    internal class Range
    {
        private readonly string _str;

        private long? _min;
        private long? _max;

        private double? _mind;
        private double? _maxd;

        private bool _includeMinBound;

        private bool _includeMaxBound;

        public Range(string strRange)
        {
            _str = strRange.Trim();
        }

        public string RawStr => _str;

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

        public void Compile()
        {
            void ThrowError()
            {
                throw new Exception($"range定义不合法");
            }

            if (long.TryParse(_str, out long value))
            {
                // size=xxxx
                _min = _max = value;
                _mind = _maxd = value;
                _includeMinBound = _includeMaxBound = true;
                return;
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


        public bool CheckInLongRange(long x)
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

        public bool CheckInDoubleRange(double x)
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
    }
}
