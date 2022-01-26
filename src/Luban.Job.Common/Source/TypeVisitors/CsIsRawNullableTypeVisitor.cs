using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsIsRawNullableTypeVisitor : AllFalseVisitor
    {
        public static CsIsRawNullableTypeVisitor Ins { get; } = new CsIsRawNullableTypeVisitor();

        public override bool Accept(TString type)
        {
            return true;
        }

        public override bool Accept(TText type)
        {
            return true;
        }

        public override bool Accept(TBytes type)
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

        public override bool Accept(TBean type)
        {
            return true;
        }
    }
}
