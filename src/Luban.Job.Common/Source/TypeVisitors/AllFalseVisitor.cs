using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public abstract class AllFalseVisitor : DecoratorFuncVisitor<bool>
    {
        public override bool DoAccept(TType type)
        {
            return false;
        }
    }
}
