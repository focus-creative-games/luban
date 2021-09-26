using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsDefineTypeName : DecoratorFuncVisitor<string>
    {
        public static CsDefineTypeName Ins { get; } = new CsDefineTypeName();

        public override string DoAccept(TType type)
        {
            return type.IsNullable ? (type.Apply(CsUnderingDefineTypeName.Ins) + "?") : type.Apply(CsUnderingDefineTypeName.Ins);
        }

        public override string Accept(TString type)
        {
            return type.Apply(CsUnderingDefineTypeName.Ins);
        }

        public override string Accept(TText type)
        {
            return type.Apply(CsUnderingDefineTypeName.Ins);
        }

        public override string Accept(TBean type)
        {
            return type.Apply(CsUnderingDefineTypeName.Ins);
        }
    }
}
