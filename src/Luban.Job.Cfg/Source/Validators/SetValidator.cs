using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Validators
{
    [Validator("set")]
    class SetValidator : IValidator
    {
        private readonly TType _type;

        private readonly HashSet<DType> _datas;

        private readonly string _valueSetStr;

        public SetValidator(TType ttype, string param)
        {
            _type = ttype;
            _datas = new HashSet<DType>();
            _valueSetStr = DefUtil.TrimBracePairs(param);
        }

        public void Compile(DefFieldBase def)
        {
            foreach (var p in _valueSetStr.Split(',', ';'))
            {
                _datas.Add(_type.Apply(StringDataCreator.Ins, p));
            }
        }

        public void Validate(ValidatorContext ctx, TType type, DType data)
        {
            if (type.IsNullable && data == null)
            {
                return;
            }
            if (!_datas.Contains(data))
            {
                ctx.Assembly.Agent.Error("记录 {0}:{1} (来自文件:{2}) 值不在set:{3}中", ValidatorContext.CurrentRecordPath, data,
                    ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source, _valueSetStr);
            }
        }
    }
}
