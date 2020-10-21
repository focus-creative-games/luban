using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public abstract class AllTrueVisitor : DecoratorFuncVisitor<bool>
    {
        public override bool DoAccept(TType type)
        {
            return true;
        }
    }
}
