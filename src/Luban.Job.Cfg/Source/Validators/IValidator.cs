using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;

namespace Luban.Job.Cfg.Validators
{
    public interface IValidator
    {
        void Compile(DefField def);

        void Validate(ValidatorContext ctx, DType data, bool nullable);
    }
}
