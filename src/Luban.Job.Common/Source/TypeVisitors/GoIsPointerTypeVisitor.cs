using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class GoIsPointerTypeVisitor : DecoratorFuncVisitor<bool>
    {
        public static GoIsPointerTypeVisitor Ins { get; } = new();

        public override bool DoAccept(TType type)
        {
            return type.IsNullable;
        }

        public override bool Accept(TBytes type)
        {
            return false;
        }

        public override bool Accept(TBean type)
        {
            return false;
        }

        public override bool Accept(TArray type)
        {
            return false;
        }

        public override bool Accept(TList type)
        {
            return false;
        }

        public override bool Accept(TSet type)
        {
            return false;
        }

        public override bool Accept(TMap type)
        {
            return false;
        }
    }
}
