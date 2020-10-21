using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class IsMultiData : AllFalseVisitor
    {
        public static IsMultiData Ins { get; } = new IsMultiData();

        public override bool Accept(TBytes type)
        {
            return true;
        }


        public override bool Accept(TBean type)
        {
            return true;
        }

        public override bool Accept(TArray type)
        {
            return true;
        }

        public override bool Accept(TList type)
        {
            return true;
        }

        public override bool Accept(TSet type)
        {
            return true;
        }

        public override bool Accept(TMap type)
        {
            return true;
        }

        public override bool Accept(TVector2 type)
        {
            return true;
        }

        public override bool Accept(TVector3 type)
        {
            return true;
        }

        public override bool Accept(TVector4 type)
        {
            return true;
        }
    }
}
