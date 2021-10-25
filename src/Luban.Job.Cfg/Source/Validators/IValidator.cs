using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;

namespace Luban.Job.Cfg.Validators
{
    public interface IValidator : IProcessor
    {
        void Validate(ValidatorContext ctx, TType type, DType data);
    }
}
