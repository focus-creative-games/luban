using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoTypeNameVisitor : DecoratorFuncVisitor<string>
    {
        public static GoTypeNameVisitor Ins { get; } = new GoTypeNameVisitor();

        public override string DoAccept(TType type)
        {
            var s = type.Apply(GoTypeUnderingNameVisitor.Ins);
            return type.Apply(IsGoPointerTypeVisitor.Ins) ? "*" + s : s;
        }
    }
}
