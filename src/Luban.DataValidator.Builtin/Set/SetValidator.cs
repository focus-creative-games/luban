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

﻿using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Set;

[Validator("set")]
public class SetValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private LongDataSet _longSet;
    private StringDataSet _stringSet;

    private Func<DType, long> _longGetter;
    private Func<DType, string> _stringGetter;

    private string _valueSetStr;

    public SetValidator()
    {
    }

    public override void Compile(DefField field, TType type)
    {
        _valueSetStr = Args;
        switch (type)
        {
            case TByte:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DByte)d).Value;
                break;
            }
            case TShort:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DShort)d).Value;
                break;
            }
            case TInt:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DInt)d).Value;
                break;
            }
            case TLong:
            {
                _longSet = new LongDataSet(Args);
                _longGetter = d => ((DLong)d).Value;
                break;
            }
            case TEnum etype:
            {
                DefEnum enumType = etype.DefEnum;
                _longSet = new LongDataSet(Args.Split(',').Select(s => (long)enumType.GetValueByNameOrAlias(s)));
                _longGetter = d => ((DEnum)d).Value;
                break;
            }
            case TString:
            {
                _stringSet = new StringDataSet(Args);
                _stringGetter = d => ((DString)d).Value;
                break;
            }
            default:
            {
                throw new Exception($"set not support type:{type} field:{field}");
            }
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        if ((_longSet != null && !_longSet.Contains(_longGetter(data))) || (_stringSet != null && !_stringSet.Contains(_stringGetter(data))))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) 值不在set:{}中", RecordPath, data, Source, _valueSetStr);
            GenerationContext.Current.LogValidatorFail(this);
        }
    }
}
