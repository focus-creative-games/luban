namespace Luban.DataValidator.Builtin.Range;

public class DoubleRange
{
    private readonly string _str;

    private readonly double? _min;
    private readonly double? _max;

    private readonly bool _includeMinBound;
    private readonly bool _includeMaxBound;

    public DoubleRange(string strRange)
    {
        _str = strRange.Trim();

        if (double.TryParse(_str, out var value))
        {
            _min = _max = value;
            _includeMinBound = _includeMaxBound = true;
            return;
        }

        if (_str.Length <= 2)
        {
            throw new Exception($"range定义不合法");
        }
        switch (_str[0])
        {
            case '[':
                _includeMinBound = true;
                break;
            case '(':
                _includeMinBound = false;
                break;
            default:
                throw new Exception($"range定义不合法");
        }
        switch (_str[^1])
        {
            case ']':
                _includeMaxBound = true;
                break;
            case ')':
                _includeMaxBound = false;
                break;
            default:
                throw new Exception($"range定义不合法");
        }

        var pars = _str[1..^1].Split(',');
        if (pars.Length != 2)
        {
            throw new Exception($"range定义不合法");
        }

        bool p1 = TryParse(pars[0], ref _min);
        bool p2 = TryParse(pars[1], ref _max);

        if (!p1 || !p2)
        {
            throw new Exception($"range定义不合法");
        }
        if (_min != null && _min != (long)_min)
        {
            _min -= 1e-6;
        }
        if (_max != null && _max != (long)_max)
        {
            _max += 1e-6;
        }
    }

    public string RawStr => _str;

    private bool TryParse(string s, ref double? x)
    {
        s = s.Trim();
        if (string.IsNullOrEmpty(s))
        {
            x = null;
            return true;
        }
        if (double.TryParse(s, out var v))
        {
            x = v;
            return true;
        }
        return false;
    }


    public bool CheckInRange(double x)
    {
        if (_min is { } m && (_includeMinBound ? m > x : m >= x))
        {
            return false;
        }
        if (_max is { } n && (_includeMaxBound ? n < x : n <= x))
        {
            return false;
        }
        return true;
    }
}
