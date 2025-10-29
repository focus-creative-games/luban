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
using Luban.DataValidator.Builtin.Range;
using Luban.Defs;
using Luban.Types;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Collection;

[Validator("size")]
public class SizeValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private LongRange _range;

    private Func<DType, long> _sizeGetter;

    public SizeValidator()
    {

    }

    public override void Compile(DefField field, TType type)
    {
        _range = new LongRange(Args);
        _sizeGetter = type switch
        {
            TList => d => ((DList)d).Datas.Count,
            TSet => d => ((DSet)d).Datas.Count,
            TMap => d => ((DMap)d).DataMap.Count,
            TArray => d => ((DArray)d).Datas.Count,
            _ => throw new Exception($"type:{type} field:{field} not support size validator"),
        };
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        long size = _sizeGetter(data);
        if (!_range.CheckInRange(size))
        {
            s_logger.Error("记录 {}:{} (来自文件:{}) size:{},但要求为 {} ", DataValidatorContext.CurrentRecordPath, data, Source, size, _range.RawStr);
            GenerationContext.Current.LogValidatorFail(this);
        }
    }
}
