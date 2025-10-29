// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

﻿namespace Luban.DataValidator.Builtin.Range;

public class LongRange
{
    private readonly string _str;

    private readonly long? _min;
    private readonly long? _max;

    private readonly bool _includeMinBound;
    private readonly bool _includeMaxBound;

    public LongRange(string strRange)
    {
        _str = strRange.Trim();

        if (long.TryParse(_str, out long value))
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
        if (long.TryParse(s, out var v))
        {
            x = v;
            return true;
        }
        return false;
    }


    public bool CheckInRange(long x)
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
