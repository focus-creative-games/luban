using Luban.Datas;
using Luban.DataValidator.Builtin.Range;
using Luban.Defs;
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
            throw new Exception($" field:{field} index:{Args} validator not support type:{type}");
        }
        if (elementType is not TBean bean)
        {
            throw new Exception($"field:{field} index:{Args} type:{elementType} validator only support bean type");
        }

        if (!bean.DefBean.TryGetField(Args, out var indexField, out _fieldIndex))
        {
            throw new Exception($"field:{field} index:{Args} not exist in bean:{bean.DefBean.FullName}");
        }

        if (!indexField.NeedExport())
        {
            throw new Exception($"field:{field} index:{Args} in bean:{bean.DefBean.FullName} is not export");
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
                throw new Exception("not possible");
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
                s_logger.Error("记录 {}:{} (来自文件:{}) index:{} value:{} 重复", DataValidatorContext.CurrentRecordPath, data, Source, Args, fieldData);
                GenerationContext.Current.LogValidatorFail(this);
            }
        }
    }
}
