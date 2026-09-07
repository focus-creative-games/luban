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

using Luban.Datas;
using Luban.Defs;
using Luban.Diagnostics;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Collection;

[Validator("index")]
public class IndexValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private int _fieldIndex;

    public IndexValidator()
    {

    }

    public override void Compile(DefField field, TType type)
    {
        TType elementType = type.ElementType;
        if (elementType == null || type is TMap)
        {
            throw new LubanException("error.validator.index.unsupported_type", field, Args, type);
        }
        if (elementType is not TBean bean)
        {
            throw new LubanException("error.validator.index.not_bean", field, Args, elementType);
        }

        if (!bean.DefBean.TryGetField(Args, out var indexField, out _fieldIndex))
        {
            throw new LubanException("error.validator.index.not_exist", field, Args, bean.DefBean.FullName);
        }

        if (!indexField.NeedExport())
        {
            throw new LubanException("error.validator.index.not_export", field, Args, bean.DefBean.FullName);
        }
    }

    private IEnumerable<DType> GetElements(DType data)
    {
        switch (data)
        {
            case DArray array:
                return array.Datas;
            case DList list:
                return list.Datas;
            case DSet dset:
                return dset.Datas;
            default:
                throw new LubanException("error.internal.not_possible");
        }
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        var values = new HashSet<DType>();
        foreach (var ele in GetElements(data))
        {
            DType fieldData = ((DBean)ele).Fields[_fieldIndex];
            if (fieldData != null && !values.Add(fieldData))
            {
                s_logger.Error(MessageCatalog.Format("error.validator.index.duplicate", DataValidatorContext.CurrentRecordPath, data, Source, Args, fieldData));
                GenerationContext.Current.LogValidatorFail(this);
            }
        }
    }
}
